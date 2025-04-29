using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StorkDorkMain.Data;
using StorkDorkMain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using StorkDorkMain.DAL.Abstract;
using Microsoft.EntityFrameworkCore.Diagnostics;
using StorkDorkMain.DAL.Concrete;

//commenting here because apparently
namespace StorkDork.Controllers
{
    

    public class BirdLogController : Controller
    {
        private readonly IMilestoneRepository _milestoneRepo; //for SD-71-Milestone-Update
        private readonly StorkDorkContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISDUserRepository _sdUserRepository;
        

        public BirdLogController(StorkDorkContext context, UserManager<IdentityUser> userManager, ISDUserRepository sdUserRepository, IMilestoneRepository milestoneRepo)
        {
            _context = context;
            _userManager = userManager;
            _sdUserRepository = sdUserRepository;
            _milestoneRepo = milestoneRepo;

            
        }

        [HttpGet("birds/search")]
        public async Task<IActionResult> GetBirdSpecies(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Json(new List<object>()); // Return an empty list if the term is empty
            }

            if(term.ToLower() == "n/a")
            {
                var naOption = new
                {
                    id = (int?)null,
                    text = "N/A",
                    scientificName = "Not Applicable"
                };
                return Json(new List<object> { naOption });
            }

            // Fetch birds from the database whose CommonName contains the search term
            var birds = await _context.Birds
                .Where(b => b.CommonName.ToLower().Contains(term.ToLower()))
                .Select(b => new 
                    { 
                        id = b.Id,
                        text = b.CommonName,
                        scientificName = b.ScientificName
                        
                    })
                    .Take(10)
                    .ToListAsync();
   
            return Json(birds);
        }

        // GET: BirdLog
        public async Task<IActionResult> Index(string sortOrder, int? birdId, int? locationId, string filterBird, string[] selectedBirds)
        {
            //This is to check if the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized("User is not authenticated"); 
            }

            //To get the current user's SdUser
            var sdUser = await _sdUserRepository.GetSDUserByIdentity(User);
            if (sdUser == null)
            {
                return NotFound("User not found");
            }



            //To get sightings based on the logged on user
            var sightingsQuery = _context.Sightings
                .Include(s => s.Bird)
                .Include(s => s.SdUser)
                .Where(s => s.SdUserId == sdUser.Id)
                .AsQueryable();
            
          

            // Apply bird filter if a birdId is provided
            if (birdId.HasValue)
            {
                sightingsQuery = sightingsQuery.Where(s => s.Bird != null && s.Bird.Id == birdId); // Filter sightings by birdId
                ViewBag.SelectedBirdCount = await sightingsQuery.CountAsync(); // Store the count of filtered sightings
                ViewBag.SelectedBirdName = await _context.Birds
                    .Where(b => b.Id == birdId)
                    .Select(b => b.CommonName)
                    .FirstOrDefaultAsync(); // Fetch the common name of the selected bird
            }


            ViewBag.UserName = sdUser.FirstName;
            // Predefined dictionary of PNW locations with their coordinates and names
            ViewBag.PnwLocations = new Dictionary<string, string>
            {
                { "48.4244,-122.3358", "Skagit Valley, WA" },
                { "46.8797,-121.7269", "Mount Rainier National Park, WA" },
                { "47.6573,-122.4057", "Discovery Park, Seattle, WA" },
                { "47.0726,-122.7175", "Nisqually National Wildlife Refuge, WA" },
                { "47.8601,-123.9343", "Olympic National Park (Hoh Rainforest), WA" },
                { "45.7156,-122.7745", "Sauvie Island, OR" },
                { "42.9778,-118.9097", "Malheur National Wildlife Refuge, OR" },
                { "42.8684,-122.1685", "Crater Lake National Park, OR" },
                { "45.9190,-123.9740", "Ecola State Park, OR" },
                { "42.1561,-121.7381", "Klamath Basin, OR" },
                { "49.0456,-123.0586", "Boundary Bay, BC" },
                { "49.3043,-123.1443", "Stanley Park, Vancouver, BC" },
                { "49.1167,-123.1500", "Reifel Migratory Bird Sanctuary, BC" },
                { "48.7500,-125.5000", "Pacific Rim National Park, BC" },
                { "49.5000,-119.5833", "Okanagan Valley, BC" },
                { "47.5000,-116.8000", "Lake Coeur d’Alene, ID" },
                { "43.3000,-112.0000", "Camas National Wildlife Refuge, ID" },
                { "44.3611,-111.4550", "Harriman State Park, ID" }
            };

            ViewBag.SortOrder = sortOrder;
            ViewBag.Birds = await _context.Birds.ToListAsync();
            
            

            ViewBag.FamilyCommonNames = await _context.Birds
                .Where(b => b.FamilyCommonName != null) // Ensure no null values
                .Select(b => b.FamilyCommonName)
                .Distinct()
                .ToListAsync();

            ViewBag.FamilyScientificNames = await _context.Birds
                .Where(b => b.FamilyScientificName != null) // Ensure no null values
                .Select(b => b.FamilyScientificName)
                .Distinct()
                .ToListAsync();

            
            // Apply bird name filter if filterBird is provided
            if (!string.IsNullOrEmpty(filterBird))
            {
                sightingsQuery =  sightingsQuery
                    .Where(s => s.Bird != null && s.Bird.CommonName != null && s.Bird.CommonName
                    .Contains(filterBird)); // Filter sightings by bird name
            }
            
            sightingsQuery = ApplySorting(sortOrder, sightingsQuery);


            

            // Sorting options for the sightings
            var sortOptions = new Dictionary<string, Func<IQueryable<Sighting>,IQueryable<Sighting>>>
            {
                
                // Sort by date (newest to oldest), nulls at the bottom
                { "date-asc", q => q.OrderBy(s => s.Date == null).ThenByDescending(s => s.Date) },

                // Sort by date (oldest to newest), nulls at the bottom
                { "date-desc", q => q.OrderBy(s => s.Date == null).ThenBy(s => s.Date) },

                // Sort by null dates first, then by date (newest to oldest)
                { "date-null", q => q.OrderBy(s => s.Date == null ? 0 : 1).ThenByDescending(s => s.Date) },

                
                // Sort by bird name (A-Z), nulls at the bottom
                { "bird", q => q.OrderBy(s => s.Bird == null)
                                .ThenBy(s => s.Bird != null ? s.Bird.CommonName : string.Empty) },

                // Sort by bird name (Z-A), nulls at the bottom
                { "bird-desc", q => q.OrderBy(s => s.Bird == null)
                                .ThenByDescending(s => s.Bird != null ? s.Bird.CommonName : string.Empty) },

                // Sort by null bird names first, then by bird name (A-Z)
                { "bird-null", q => q.OrderBy(s => s.Bird == null ? 0 : 1)
                                    .ThenBy(s => s.Bird != null ? s.Bird.CommonName : string.Empty) },

                // Sort by location (A-Z), nulls at the bottom
                { "location", q => q.OrderBy(s => GetLocationName(s.Latitude, s.Longitude) == null).ThenBy(s => GetLocationName(s.Latitude, s.Longitude)) },

                // Sort by location (Z-A), nulls at the bottom
                { "location-desc", q => q.OrderBy(s => GetLocationName(s.Latitude, s.Longitude) == null).ThenByDescending(s => GetLocationName(s.Latitude, s.Longitude)) },

                // Sort by null locations first, then by location (A-Z)
                { "location-null", q => q.OrderBy(s => GetLocationName(s.Latitude, s.Longitude) == null ? 0 : 1).ThenBy(s => GetLocationName(s.Latitude, s.Longitude)) }

            };
            

            // Apply sorting if a valid sortOrder is provided
            if (!string.IsNullOrEmpty(sortOrder) && sortOptions.ContainsKey(sortOrder))
            {
    
                if (sortOrder.StartsWith("location"))
                {
                    var sightingsList = await sightingsQuery.ToListAsync();

                    switch (sortOrder)
                    {
                        case "location":
                        sightingsList = sightingsList
                            .OrderBy(s => GetLocationName(s.Latitude, s.Longitude) == null)
                            .ThenBy(s => GetLocationName(s.Latitude, s.Longitude))
                            .ToList();
                        break;

                        case "location-desc":
                            sightingsList = sightingsList
                                .OrderBy(s => GetLocationName(s.Latitude, s.Longitude) == null)
                                .ThenByDescending(s => GetLocationName(s.Latitude, s.Longitude))
                                .ToList();
                            break;

                        case "location-null":
                            sightingsList = sightingsList
                                .OrderBy(s => GetLocationName(s.Latitude, s.Longitude) == null ? 0 : 1)
                                .ThenBy(s => GetLocationName(s.Latitude, s.Longitude))
                                .ToList();
                            break;
                    }
                    return View(sightingsList);
                }
                else
                {
                    sightingsQuery = sortOptions[sortOrder](sightingsQuery); // Apply the selected sorting option
                }
    
            }

            // A predefined list of common PNW bird sighting locations with longitude and latitde coordinates
            ViewBag.PnwLocations = new Dictionary<string, string>
            {
                { "48.4244,-122.3358", "Skagit Valley, WA" },
                { "46.8797,-121.7269", "Mount Rainier National Park, WA" },
                { "47.6573,-122.4057", "Discovery Park, Seattle, WA" },
                { "47.0726,-122.7175", "Nisqually National Wildlife Refuge, WA" },
                { "47.8601,-123.9343", "Olympic National Park (Hoh Rainforest), WA" },
                { "45.7156,-122.7745", "Sauvie Island, OR" },
                { "42.9778,-118.9097", "Malheur National Wildlife Refuge, OR" },
                { "42.8684,-122.1685", "Crater Lake National Park, OR" },
                { "45.9190,-123.9740", "Ecola State Park, OR" },
                { "42.1561,-121.7381", "Klamath Basin, OR" },
                { "49.0456,-123.0586", "Boundary Bay, BC" },
                { "49.3043,-123.1443", "Stanley Park, Vancouver, BC" },
                { "49.1167,-123.1500", "Reifel Migratory Bird Sanctuary, BC" },
                { "48.7500,-125.5000", "Pacific Rim National Park, BC" },
                { "49.5000,-119.5833", "Okanagan Valley, BC" },
                { "47.5000,-116.8000", "Lake Coeur d’Alene, ID" },
                { "43.3000,-112.0000", "Camas National Wildlife Refuge, ID" },
                { "44.3611,-111.4550", "Harriman State Park, ID" }
            };
            

           
            var sightings = await sightingsQuery.ToListAsync();
            return View(await sightingsQuery.ToListAsync());

           
        }
        // Helper method to get location name from latitude and longitude
        private string? GetLocationName(decimal? latitude, decimal? longitude)
        {
            if (latitude == null || longitude == null)
            {
                return null;
            }
            
            // Return the location name if the key exists in ViewBag.PnwLocations, otherwise return null
            string key = $"{latitude.Value:0.0000},{longitude.Value:0.0000}";
            return ViewBag.PnwLocations.ContainsKey(key) ? ViewBag.PnwLocations[key] : null;
        }
        
        

        // GET: BirdLog/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sighting = await _context.Sightings
                .Include(s => s.Bird)
                .Include(s => s.SdUser)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sighting == null)
            {
                return NotFound();
            }

              // A predefined list of common PNW bird sighting locations with longitude and latitde coordinates
            ViewBag.PnwLocations = new Dictionary<string, string>
            {
                { "48.4244,-122.3358", "Skagit Valley, WA" },
                { "46.8797,-121.7269", "Mount Rainier National Park, WA" },
                { "47.6573,-122.4057", "Discovery Park, Seattle, WA" },
                { "47.0726,-122.7175", "Nisqually National Wildlife Refuge, WA" },
                { "47.8601,-123.9343", "Olympic National Park (Hoh Rainforest), WA" },
                { "45.7156,-122.7745", "Sauvie Island, OR" },
                { "42.9778,-118.9097", "Malheur National Wildlife Refuge, OR" },
                { "42.8684,-122.1685", "Crater Lake National Park, OR" },
                { "45.9190,-123.9740", "Ecola State Park, OR" },
                { "42.1561,-121.7381", "Klamath Basin, OR" },
                { "49.0456,-123.0586", "Boundary Bay, BC" },
                { "49.3043,-123.1443", "Stanley Park, Vancouver, BC" },
                { "49.1167,-123.1500", "Reifel Migratory Bird Sanctuary, BC" },
                { "48.7500,-125.5000", "Pacific Rim National Park, BC" },
                { "49.5000,-119.5833", "Okanagan Valley, BC" },
                { "47.5000,-116.8000", "Lake Coeur d’Alene, ID" },
                { "43.3000,-112.0000", "Camas National Wildlife Refuge, ID" },
                { "44.3611,-111.4550", "Harriman State Park, ID" }
            };

            return View(sighting);
        }




          // GET: BirdLog/Create
        public async Task<IActionResult> Create(string? searchTerm = null, string? commonName = null)
        {
            if (!string.IsNullOrWhiteSpace(commonName))
            {
                // Pass the common name to the view to prefill the field
                ViewBag.PrefilledBirdName = commonName;
            }
                // Fetch birds if a search term is provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var birds = _context.Birds
                    .Where(b => b.CommonName.ToLower().Contains(searchTerm.ToLower()))
                    .Select(b => new Bird
                    {
                        Id = b.Id,
                        CommonName = b.CommonName,
                        ScientificName = b.ScientificName
                    })
                    .Take(10)
                    .ToList();

                ViewBag.SearchResults = birds; // Pass the search results to the view
                ViewBag.SearchTerm = searchTerm; // Pass the search term to the view
                ViewBag.PnwLocations = GetPnwLocations();
            }

            var currentSdUser = await _sdUserRepository.GetSDUserByIdentity(User);
            if (currentSdUser == null)
            {
                Console.WriteLine("Error: No SdUser found for the logged-in IdentityUser.");
                return RedirectToAction("Index");

            }



           //get the current user id
            ViewBag.SdUserId = currentSdUser.Id;

            // Populate ViewBag with birds
            ViewBag.Birds = new SelectList(_context.Birds, "Id", "CommonName");

            
            ViewBag.PnwLocations = new List<SelectListItem>
            {
                new SelectListItem { Text = "Skagit Valley, WA", Value = "48.4244,-122.3358" },
                new SelectListItem { Text = "Mount Rainier National Park, WA", Value = "46.8797,-121.7269" },
                new SelectListItem { Text = "Discovery Park, Seattle, WA", Value = "47.6573,-122.4057" },
                new SelectListItem { Text = "Nisqually National Wildlife Refuge, WA", Value = "47.0726,-122.7175" },
                new SelectListItem { Text = "Olympic National Park (Hoh Rainforest), WA", Value = "47.8601,-123.9343" },
                new SelectListItem { Text = "Sauvie Island, OR", Value = "45.7156,-122.7745" },
                new SelectListItem { Text = "Malheur National Wildlife Refuge, OR", Value = "42.9778,-118.9097" },
                new SelectListItem { Text = "Crater Lake National Park, OR", Value = "42.8684,-122.1685" },
                new SelectListItem { Text = "Ecola State Park, OR", Value = "45.9190,-123.9740" },
                new SelectListItem { Text = "Klamath Basin, OR", Value = "42.1561,-121.7381" },
                new SelectListItem { Text = "Boundary Bay, BC", Value = "49.0456,-123.0586" },
                new SelectListItem { Text = "Stanley Park, Vancouver, BC", Value = "49.3043,-123.1443" },
                new SelectListItem { Text = "Reifel Migratory Bird Sanctuary, BC", Value = "49.1167,-123.1500" },
                new SelectListItem { Text = "Pacific Rim National Park, BC", Value = "48.7500,-125.5000" },
                new SelectListItem { Text = "Okanagan Valley, BC", Value = "49.5000,-119.5833" },
                new SelectListItem { Text = "Lake Coeur d’Alene, ID", Value = "47.5000,-116.8000" },
                new SelectListItem { Text = "Camas National Wildlife Refuge, ID", Value = "43.3000,-112.0000" },
                new SelectListItem { Text = "Harriman State Park, ID", Value = "44.3611,-111.4550" }

            };



            
            return View();
        }

        // POST: BirdLog/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SdUserId,BirdId,Date,Latitude,Longitude,Notes")] Sighting sightings) //IFormFile photoFile)
        {


            var currentSdUser = await _sdUserRepository.GetSDUserByIdentity(User);


            if (currentSdUser == null)

            {
                ModelState.AddModelError("", "User not found");
                return RedirectToAction("Index");
            }

            sightings.SdUserId = currentSdUser.Id;

            var selectedLocation = Request.Form["PnwLocation"];
            if (selectedLocation == "0")
            {
                sightings.Latitude = null;
                sightings.Longitude = null;
            }
            else if (string.IsNullOrEmpty(selectedLocation))
            {

                ModelState.AddModelError("PnwLocation", "Please select a location or select N/A");
            }
             // Validate bird (always check, even if location is provided)
            if (sightings.BirdId == null)
            {
                ModelState.AddModelError("BirdId", "Please select a bird or choose N/A.");
            }

   
            // Check if both Bird and Location are left blank (not even N/A)
            if (sightings.BirdId == null && string.IsNullOrEmpty(selectedLocation))
            {
                ModelState.AddModelError("BirdId", "Please select a bird or choose N/A.");
                ModelState.AddModelError("PnwLocation", "Please select a location or choose N/A.");
            }

            if (!sightings.Date.HasValue)
            {
                sightings.Date = DateTime.UtcNow;
            }

           //if (photoFile != null && photoFile.Length > 0)
           //{
                // Validate file
            //   if (photoFile.Length > 5 * 1024 * 1024) // 5MB
           //     {
              //     ModelState.AddModelError("", "File size exceeds 5MB limit");
            //    }
            //    else if (!photoFile.ContentType.StartsWith("image/")) 
            //    {
            //        ModelState.AddModelError("", "Only image files are allowed");
             //   }

           // }


            if (ModelState.IsValid)
            {
                bool hasPhoto = false;
             
                try
                {

                    //if (photoFile != null && photoFile.Length > 0)
                  //  {
                 //       using var memoryStream = new MemoryStream();
                //        await photoFile.CopyToAsync(memoryStream);
                 //       sightings.PhotoData = memoryStream.ToArray();
                 //       sightings.PhotoContentType = photoFile.ContentType;
                 //       hasPhoto = true;
                 //   }   
                   // else
                  //  {
                 ////       sightings.PhotoData = Array.Empty<byte>();
                   //     sightings.PhotoContentType = string.Empty;
                  //      Console.WriteLine("No photo file uploaded.");

                   // }




                    _context.Add(sightings);
                    await _context.SaveChangesAsync();

                    var milestone = await _context.Milestone
                        .FirstOrDefaultAsync(m => m.SDUserId == currentSdUser.Id);

                    if (milestone == null)
                    {
                        // Create new milestone if none exists
                        milestone = new Milestone 
                        { 
                            SDUserId = currentSdUser.Id,
                            SightingsMade = 1,
                            //PhotosContributed = hasPhoto ? 1 : 0
                        };
                        _context.Add(milestone);
                    }
                    else
                    {
                        // Increment existing milestone
                        milestone.SightingsMade++;
                    }

                    // Save the milestone changes
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Confirmation), new { 
                    userId = sightings.SdUserId,
                    //hasPhoto = hasPhoto

                });
            


                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving.");
                    Console.WriteLine($"Error saving sighting: {ex.Message}");
                }

            }
            
            
                
            // Debugging: Log success
            Console.WriteLine("Sighting saved successfully.");



            // If the model state is invalid, repopulate the ViewBag and return the view
            ViewBag.Birds = new SelectList(_context.Birds, "Id", "CommonName", sightings.BirdId);
                    
            
            ViewBag.PnwLocations = new List<SelectListItem>
            {
                new SelectListItem { Text = "Skagit Valley, WA", Value = "48.4244,-122.3358" },
                new SelectListItem { Text = "Mount Rainier National Park, WA", Value = "46.8797,-121.7269" },
                new SelectListItem { Text = "Discovery Park, Seattle, WA", Value = "47.6573,-122.4057" },
                new SelectListItem { Text = "Nisqually National Wildlife Refuge, WA", Value = "47.0726,-122.7175" },
                new SelectListItem { Text = "Olympic National Park (Hoh Rainforest), WA", Value = "47.8601,-123.9343" },
                new SelectListItem { Text = "Sauvie Island, OR", Value = "45.7156,-122.7745" },
                new SelectListItem { Text = "Malheur National Wildlife Refuge, OR", Value = "42.9778,-118.9097" },
                new SelectListItem { Text = "Crater Lake National Park, OR", Value = "42.8684,-122.1685" },
                new SelectListItem { Text = "Ecola State Park, OR", Value = "45.9190,-123.9740" },
                new SelectListItem { Text = "Klamath Basin, OR", Value = "42.1561,-121.7381" },
                new SelectListItem { Text = "Boundary Bay, BC", Value = "49.0456,-123.0586" },
                new SelectListItem { Text = "Stanley Park, Vancouver, BC", Value = "49.3043,-123.1443" },
                new SelectListItem { Text = "Reifel Migratory Bird Sanctuary, BC", Value = "49.1167,-123.1500" },
                new SelectListItem { Text = "Pacific Rim National Park, BC", Value = "48.7500,-125.5000" },
                new SelectListItem { Text = "Okanagan Valley, BC", Value = "49.5000,-119.5833" },
                new SelectListItem { Text = "Lake Coeur d’Alene, ID", Value = "47.5000,-116.8000" },
                new SelectListItem { Text = "Camas National Wildlife Refuge, ID", Value = "43.3000,-112.0000" },
                new SelectListItem { Text = "Harriman State Park, ID", Value = "44.3611,-111.4550" }

            };
            

            
            return View(sightings);
        }

        // GET: BirdLog/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            

            var sighting = await _context.Sightings
                .Include(s => s.Bird)
                .Include(s => s.SdUser)
                .FirstOrDefaultAsync(m => m.Id == id);

         

            if (sighting == null)
            {
                return NotFound();
            }

            var currentUser = await _sdUserRepository.GetSDUserByIdentity(User);
            if (sighting.SdUserId != currentUser?.Id)
                return Forbid();

            ViewBag.SelectedLatLong = sighting.Latitude.HasValue && sighting.Longitude.HasValue
                ? $"{sighting.Latitude},{sighting.Longitude}"
                : null;
           
            ViewBag.HasPhoto = sighting.PhotoData != null;
            ViewBag.SelectedUserId = sighting.SdUserId;
            ViewBag.SelectedUserName = sighting.SdUser?.FirstName;

            // Pass the selected bird's information to the view
            if (sighting.BirdId.HasValue)
            {
                var selectedBird = await _context.Birds
                    .FirstOrDefaultAsync(b => b.Id == sighting.BirdId);

                ViewBag.SelectedBirdName = selectedBird?.CommonName;
                ViewBag.SelectedBirdId = selectedBird?.Id;
            }
            else
            {
                ViewBag.SelectedBirdName = "N/A";
                ViewBag.SelectedBirdId = null;
            }

     

            ViewBag.PnwLocations = new List<SelectListItem>
            {
                new SelectListItem { Text = "Skagit Valley, WA", Value = "48.4244,-122.3358" },
                new SelectListItem { Text = "Mount Rainier National Park, WA", Value = "46.8797,-121.7269" },
                new SelectListItem { Text = "Discovery Park, Seattle, WA", Value = "47.6573,-122.4057" },
                new SelectListItem { Text = "Nisqually National Wildlife Refuge, WA", Value = "47.0726,-122.7175" },
                new SelectListItem { Text = "Olympic National Park (Hoh Rainforest), WA", Value = "47.8601,-123.9343" },
                new SelectListItem { Text = "Sauvie Island, OR", Value = "45.7156,-122.7745" },
                new SelectListItem { Text = "Malheur National Wildlife Refuge, OR", Value = "42.9778,-118.9097" },
                new SelectListItem { Text = "Crater Lake National Park, OR", Value = "42.8684,-122.1685" },
                new SelectListItem { Text = "Ecola State Park, OR", Value = "45.9190,-123.9740" },
                new SelectListItem { Text = "Klamath Basin, OR", Value = "42.1561,-121.7381" },
                new SelectListItem { Text = "Boundary Bay, BC", Value = "49.0456,-123.0586" },
                new SelectListItem { Text = "Stanley Park, Vancouver, BC", Value = "49.3043,-123.1443" },
                new SelectListItem { Text = "Reifel Migratory Bird Sanctuary, BC", Value = "49.1167,-123.1500" },
                new SelectListItem { Text = "Pacific Rim National Park, BC", Value = "48.7500,-125.5000" },
                new SelectListItem { Text = "Okanagan Valley, BC", Value = "49.5000,-119.5833" },
                new SelectListItem { Text = "Lake Coeur d’Alene, ID", Value = "47.5000,-116.8000" },
                new SelectListItem { Text = "Camas National Wildlife Refuge, ID", Value = "43.3000,-112.0000" },
                new SelectListItem { Text = "Harriman State Park, ID", Value = "44.3611,-111.4550" }

            };
        
            
            Console.WriteLine($"Selected Location: {ViewBag.SelectedLatLong}");

            return View(sighting);
        }

        // POST: BirdLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SdUserId,BirdId,Date,Latitude,Longitude,Notes")] Sighting sighting, IFormFile photoFile, bool removePhoto = false)
        {
            if (id != sighting.Id)
            {
                return NotFound();
            }
            
            var selectedLocation = Request.Form["PnwLocation"];

            if (sighting.BirdId == 0)
            {
                sighting.BirdId = null;

            }
            else if (sighting.BirdId == null)
            {
                ModelState.AddModelError("BirdId", "Please select a bird or enter 'N/A' if unknown");
            }

            
            

            if (selectedLocation == "0") // N/A was selected
            {
                sighting.Latitude = null;
                sighting.Longitude = null;
            }
            else if (string.IsNullOrEmpty(selectedLocation))
            {
                ModelState.AddModelError("PnwLocation","Please select a location or select N/A");
            }

            

            if (ModelState.IsValid)
            {
                try
                {
                    if (removePhoto)
                    {
                        sighting.PhotoData = null;
                        sighting.PhotoContentType = null;
                    }
                    else if (photoFile != null && photoFile.Length > 0)
                    {
                        if (photoFile.Length > 5 * 1024 * 1024) // 5MB
                        {
                            ModelState.AddModelError("photoFile", "File size exceeds 5MB limit");
                            return View(sighting);
                        }

                        if (!photoFile.ContentType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("photoFile", "Only image files are allowed");
                            return View(sighting);
                        }

                        using var memoryStream = new MemoryStream();
                        await photoFile.CopyToAsync(memoryStream);
                        sighting.PhotoData = memoryStream.ToArray();
                        sighting.PhotoContentType = photoFile.ContentType;
                    }

                    _context.Update(sighting);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { userId = sighting.SdUserId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SightingExists(sighting.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }


             if (sighting.BirdId.HasValue)
            {
                var selectedBird = await _context.Birds
                    .FirstOrDefaultAsync(b => b.Id == sighting.BirdId);

                ViewBag.SelectedBirdName = selectedBird?.CommonName;
                ViewBag.SelectedBirdId = selectedBird?.Id;
            }
            else
            {
                ViewBag.SelectedBirdName = "N/A";
                ViewBag.SelectedBirdId = null;
            }

            ViewBag.PnwLocations = new List<SelectListItem>
            {
                new SelectListItem { Text = "Skagit Valley, WA", Value = "48.4244,-122.3358" },
                new SelectListItem { Text = "Mount Rainier National Park, WA", Value = "46.8797,-121.7269" },
                new SelectListItem { Text = "Discovery Park, Seattle, WA", Value = "47.6573,-122.4057" },
                new SelectListItem { Text = "Nisqually National Wildlife Refuge, WA", Value = "47.0726,-122.7175" },
                new SelectListItem { Text = "Olympic National Park (Hoh Rainforest), WA", Value = "47.8601,-123.9343" },
                new SelectListItem { Text = "Sauvie Island, OR", Value = "45.7156,-122.7745" },
                new SelectListItem { Text = "Malheur National Wildlife Refuge, OR", Value = "42.9778,-118.9097" },
                new SelectListItem { Text = "Crater Lake National Park, OR", Value = "42.8684,-122.1685" },
                new SelectListItem { Text = "Ecola State Park, OR", Value = "45.9190,-123.9740" },
                new SelectListItem { Text = "Klamath Basin, OR", Value = "42.1561,-121.7381" },
                new SelectListItem { Text = "Boundary Bay, BC", Value = "49.0456,-123.0586" },
                new SelectListItem { Text = "Stanley Park, Vancouver, BC", Value = "49.3043,-123.1443" },
                new SelectListItem { Text = "Reifel Migratory Bird Sanctuary, BC", Value = "49.1167,-123.1500" },
                new SelectListItem { Text = "Pacific Rim National Park, BC", Value = "48.7500,-125.5000" },
                new SelectListItem { Text = "Okanagan Valley, BC", Value = "49.5000,-119.5833" },
                new SelectListItem { Text = "Lake Coeur d’Alene, ID", Value = "47.5000,-116.8000" },
                new SelectListItem { Text = "Camas National Wildlife Refuge, ID", Value = "43.3000,-112.0000" },
                new SelectListItem { Text = "Harriman State Park, ID", Value = "44.3611,-111.4550" }

            };

            return View(sighting);
        }

        // GET: BirdLog/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sighting = await _context.Sightings
                .Include(s => s.Bird)
                .Include(s => s.SdUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sighting == null)
            {
                return NotFound();
            }

            ViewBag.PnwLocations =  GetPnwLocations();

            

            return View(sighting);
        }
            private Dictionary<string, string> GetPnwLocations()
            {
                return new Dictionary<string, string>
                {
                    { "48.4244,-122.3358", "Skagit Valley, WA" },
                    { "46.8797,-121.7269", "Mount Rainier National Park, WA" },
                    { "47.6573,-122.4057", "Discovery Park, Seattle, WA" },
                    { "47.0726,-122.7175", "Nisqually National Wildlife Refuge, WA" },
                    { "47.8601,-123.9343", "Olympic National Park (Hoh Rainforest), WA" },
                    { "45.7156,-122.7745", "Sauvie Island, OR" },
                    { "42.9778,-118.9097", "Malheur National Wildlife Refuge, OR" },
                    { "42.8684,-122.1685", "Crater Lake National Park, OR" },
                    { "45.9190,-123.9740", "Ecola State Park, OR" },
                    { "42.1561,-121.7381", "Klamath Basin, OR" },
                    { "49.0456,-123.0586", "Boundary Bay, BC" },
                    { "49.3043,-123.1443", "Stanley Park, Vancouver, BC" },
                    { "49.1167,-123.1500", "Reifel Migratory Bird Sanctuary, BC" },
                    { "48.7500,-125.5000", "Pacific Rim National Park, BC" },
                    { "49.5000,-119.5833", "Okanagan Valley, BC" },
                    { "47.5000,-116.8000", "Lake Coeur d’Alene, ID" },
                    { "43.3000,-112.0000", "Camas National Wildlife Refuge, ID" },
                    { "44.3611,-111.4550", "Harriman State Park, ID" }
                };
            }
            
        

        // POST: BirdLog/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sighting = await _context.Sightings.FindAsync(id);
            if (sighting != null)
            {
                _context.Sightings.Remove(sighting);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SightingExists(int id)
        {
            return _context.Sightings.Any(e => e.Id == id);
        }

        public IActionResult Confirmation(int? userId, bool hasPhoto)
        {
            var sighting = _context.Sightings.Find(userId);
            
            // Initialize HasPhoto as boolean
            ViewBag.HasPhoto = sighting?.PhotoData != null; // This will be true or false, never null
            
            ViewBag.UserId = userId;
            ViewBag.UserName = _context.SdUsers.Find(userId)?.FirstName;
            
            return View(sighting);
        }

        [HttpGet]
        public async Task<IActionResult> GetSightings(int userId)
        {
            var sightings = await _context.Sightings
                .Where(s => s.SdUserId == userId)
                .Select(s => new
                {
                    id = s.Id,
                    date = s.Date.HasValue ? s.Date.Value.ToShortDateString() : "No date entered",
                    birdCommonName = s.Bird != null ? s.Bird.CommonName : "Bird Species Unavailable",
                    location = (s.Latitude.HasValue && s.Longitude.HasValue) 
                        ? $"{s.Latitude.Value.ToString("0.0000")}, {s.Longitude.Value.ToString("0.0000")}" 
                        : "Unknown Location",
                    notes = !string.IsNullOrEmpty(s.Notes) ? s.Notes : "No notes recorded"
                })
                .ToListAsync();

            return Json(sightings);
        }

        
        private IQueryable<Sighting> ApplySorting(string sortOrder, IQueryable<Sighting> sightingsQuery)
        {
            // Add your sorting logic here (same as in the Index action)
            switch (sortOrder)
            {
                case "date-asc":
                    return sightingsQuery.OrderBy(s => s.Date);
                case "date-desc":
                    return sightingsQuery.OrderByDescending(s => s.Date);
                case "date-null":
                    return sightingsQuery
                        .OrderBy(s => s.Date == null ? 0 : 1) // Sort nulls first
                        .ThenBy(s => s.Date); // Then sort by date

                case "bird":
                    return sightingsQuery.OrderBy(s => s.Bird.CommonName);
                case "bird-desc":
                    return sightingsQuery.OrderByDescending(s => s.Bird.CommonName);

                case "location":
                    return sightingsQuery.OrderBy(s => s.Latitude).ThenBy(s => s.Longitude);
                case "location-desc":
                    return sightingsQuery.OrderByDescending(s => s.Latitude).ThenByDescending(s => s.Longitude);
            }
            return sightingsQuery.OrderBy(s => s.Date);
        }
        
        public IActionResult GetSightingImage(int id)
        {
            var sighting = _context.Sightings.Find(id);
            if (sighting?.PhotoData != null)
            {
                return File(sighting.PhotoData, sighting.PhotoContentType);
            }
            return NotFound();
        }
            


    }
}