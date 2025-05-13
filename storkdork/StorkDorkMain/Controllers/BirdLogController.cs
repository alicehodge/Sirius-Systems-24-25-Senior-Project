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


namespace StorkDorkMain.Controllers
{
    

    public class BirdLogController : Controller
    {
        private readonly IMilestoneRepository _milestoneRepo; //for SD-71-Milestone-Update
        private readonly StorkDorkDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISDUserRepository _sdUserRepository;
        

        public BirdLogController(StorkDorkDbContext context, UserManager<IdentityUser> userManager, ISDUserRepository sdUserRepository, IMilestoneRepository milestoneRepo)
        {
            _context = context;
            _userManager = userManager;
            _sdUserRepository = sdUserRepository;
            _milestoneRepo = milestoneRepo;

            
        }

        //a method to search for birds in the bird log.
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
        //This is the main for a user to view their sightings.
        // Index.cshtml
        public async Task<IActionResult> Index(string sortOrder, int? birdId, string filterBird, string[] selectedBirds)
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

            ViewBag.SortOrder = sortOrder;
            // In the Index action
            ViewBag.FamilyCommonNames = await _context.Birds
                .Select(b => b.FamilyCommonName)
                .Distinct()
                .ToListAsync();
       

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
                { "location", q => q.OrderBy(s => s.Latitude).ThenBy(s => s.Latitude) },

                // Sort by location (Z-A), nulls at the bottom
                { "location-desc", q => q.OrderByDescending(s => s.Latitude).ThenByDescending(s => s.Longitude) },

                // Sort by null locations first, then by location (A-Z)
                { "location-null", q => q.OrderBy(s => s.Latitude == null ? 0 : 1).ThenBy(s => s.Longitude == null ? 0 : 1) }

            };
                     
            var sightings = await sightingsQuery.ToListAsync();
            return View(await sightingsQuery.ToListAsync());

           
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


            return View(sighting);
        }




        // GET: BirdLog/Create
        [HttpGet]
        [Authorize]
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

            ViewBag.DefaultLat = 45.5231; // Example default coordinates
            ViewBag.DefaultLng = -122.6765;
            ViewBag.DefaultZoom = 7;
   
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

            if ((sightings.Latitude.HasValue || sightings.Longitude.HasValue) &&
                (!sightings.Latitude.HasValue || !sightings.Longitude.HasValue))
            {
                ModelState.AddModelError("", "Both coordinates must be provided");
            }

            if (sightings.BirdId == null)
            {
                ModelState.AddModelError("BirdId", "Please select a bird.");
            }


            if (!sightings.Date.HasValue)
            {
                sightings.Date = DateTime.UtcNow;
            }

            if (!ModelState.IsValid)
            {
                // Repopulate necessary view data
                ViewBag.Birds = new SelectList(_context.Birds, "Id", "CommonName", sightings.BirdId);
                ViewBag.DefaultLat = sightings.Latitude ?? 45.5231m;
                ViewBag.DefaultLng = sightings.Longitude ?? -122.6765m;
                ViewBag.DefaultZoom = sightings.Latitude != null ? 12 : 7;
                
                // Preserve search term if needed
                ViewBag.PrefilledBirdName = Request.Form["birdSearch"];
                
                return View(sightings);
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
            ViewBag.DefaultLat  = sightings.Latitude  ?? 45.5231m;
            ViewBag.DefaultLng = sightings.Longitude ?? -122.6765m;
            ViewBag.DefaultZoom = sightings.Latitude != null ? 12 : 7;
                          
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

            return View(sighting);
        }

        // POST: BirdLog/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SdUserId,BirdId,Date,Latitude,Longitude,Notes")] Sighting sightings, IFormFile photoFile, bool removePhoto = false)
        {
            if (id != sightings.Id)
            {
                return NotFound();
            }

            var currentUser = await _sdUserRepository.GetSDUserByIdentity(User);
            if (sightings.SdUserId != currentUser?.Id)
            {
                return Forbid();
            }

            

            if (sightings.BirdId == 0)
            {
                sightings.BirdId = null;

            }
            else if (sightings.BirdId == null)
            {
                ModelState.AddModelError("BirdId", "Please select a bird");
            }

            if ((sightings.Latitude.HasValue || sightings.Longitude.HasValue) &&
                (!sightings.Latitude.HasValue || !sightings.Longitude.HasValue))
            {
                ModelState.AddModelError("", "Both coordinates must be provided");
            }

            
            
            

            if (ModelState.IsValid)
            {
                try
                {
                    if (removePhoto)
                    {
                        sightings.PhotoData = null;
                        sightings.PhotoContentType = null;
                    }
                    else if (photoFile != null && photoFile.Length > 0)
                    {
                        if (photoFile.Length > 5 * 1024 * 1024) // 5MB
                        {
                            ModelState.AddModelError("photoFile", "File size exceeds 5MB limit");
                            return View(sightings);
                        }

                        if (!photoFile.ContentType.StartsWith("image/"))
                        {
                            ModelState.AddModelError("photoFile", "Only image files are allowed");
                            return View(sightings);
                        }

                        using var memoryStream = new MemoryStream();
                        await photoFile.CopyToAsync(memoryStream);
                        sightings.PhotoData = memoryStream.ToArray();
                        sightings.PhotoContentType = photoFile.ContentType;
                    }

                    _context.Update(sightings);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { userId = sightings.SdUserId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SightingExists(sightings.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                
            }


             if (sightings.BirdId.HasValue)
            {
                var selectedBird = await _context.Birds
                    .FirstOrDefaultAsync(b => b.Id == sightings.BirdId);

                ViewBag.SelectedBirdName = selectedBird?.CommonName;
                ViewBag.SelectedBirdId = selectedBird?.Id;
            }
            else
            {
                ViewBag.SelectedBirdName = "N/A";
                ViewBag.SelectedBirdId = null;
            }

            return View(sightings);
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


        
            return View(sighting);
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
        

        // this is the confirmation page so when a user successfully creates a logged sighting, they will be taken here
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