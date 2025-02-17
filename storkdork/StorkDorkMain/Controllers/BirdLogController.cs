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

namespace StorkDork.Controllers
{
    public class BirdLogController : Controller
    {
        private readonly StorkDorkContext _context;

        public BirdLogController(StorkDorkContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetBirdSpecies(string term)
        {
            // Fetch birds from the database whose CommonName contains the search term
            var birds = await _context.Birds
                .Where(b => b.CommonName.Contains(term))
                .Select(b => new { id = b.Id, text = b.CommonName })
                .Distinct() // Ensure unique results
                .ToListAsync();
                // Store the fetched birds in ViewBag for use in the view
                ViewBag.Birds = birds;

    
            return Json(birds);
        }

        // GET: BirdLog
        public async Task<IActionResult> Index(string sortOrder, int? birdId, int? locationId, string filterBird)
        {
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

            
            var sightings = _context.Sightings
                .Include(s => s.Bird)  
                .Include(s => s.SdUser) 
                .AsQueryable(); // Allows dynamic sorting


            // Apply bird filter if a birdId is provided
            if (birdId.HasValue)
            {
                sightings = sightings.Where(s => s.Bird != null && s.Bird.Id == birdId); // Filter sightings by birdId
                ViewBag.SelectedBirdCount = await sightings.CountAsync(); // Store the count of filtered sightings
                ViewBag.SelectedBirdName = await _context.Birds
                    .Where(b => b.Id == birdId)
                    .Select(b => b.CommonName)
                    .FirstOrDefaultAsync(); // Fetch the common name of the selected bird
            }
            
            // Apply bird name filter if filterBird is provided
            if (!string.IsNullOrEmpty(filterBird))
            {
                sightings =  sightings.Where(s => s.Bird != null && s.Bird.CommonName != null && s.Bird.CommonName.Contains(filterBird)); // Filter sightings by bird name
            }

            // Convert the filtered sightings to a list
            var sightingList =  await sightings.ToListAsync(); 

            // Sorting options for the sightings
            var sortOptions = new Dictionary<string, Func<IQueryable<Sighting>,IQueryable<Sighting>>>
            {
                
                // Sort by date (newest to oldest), nulls at the bottom
                { "date-desc", q => q.OrderByDescending(s => s.Date != null).ThenByDescending(s => s.Date) },

                // Sort by date (oldest to newest), nulls at the bottom
                { "date-asc", q => q.OrderByDescending(s => s.Date != null).ThenBy(s => s.Date) },

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
                    var sightingsList = await sightings.ToListAsync();

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
                    sightings = sortOptions[sortOrder](sightings); // Apply the selected sorting option
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
            
            return View(await sightings.ToListAsync());
           
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
                .Include(s => s.Sduser)
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
        public IActionResult Create()
        {
            //To populate ViewBag and ViewData
            ViewData["BirdId"] = new SelectList(_context.Birds, "Id", "CommonName");
            ViewData["SduserId"] = new SelectList(_context.Sdusers, "Id", "Id");

            

            
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
        public async Task<IActionResult> Create([Bind("Id,SduserId,BirdId,Date,Latitude,Longitude,Notes")] Sighting sighting)
        {
            var selectedLocation = Request.Form["PnwLocation"];

            // Check if SduserId is empty
            if (sighting.SduserId == 0)
            {
                ModelState.AddModelError("SduserId", "Please select a user.");
            }
            
            // To check if BirdId is empty or "N/A"
            if (sighting.BirdId == 0)
            {
                sighting.BirdId = null;
            }
        

            if (selectedLocation == "0")
            {
                sighting.Latitude = null;
                sighting.Longitude = null;
            }
            else if (string.IsNullOrEmpty(selectedLocation))
            {
                ModelState.AddModelError("PnwLocation", "Please select a location or select N/A");
            }

            // Check if both Bird and Location are left blank (not even N/A)
            if ((sighting.BirdId == null && string.IsNullOrEmpty(selectedLocation)))
            {
                ModelState.AddModelError("BirdId", "Please select a bird or choose N/A.");
                ModelState.AddModelError("PnwLocation", "Please select a location or choose N/A.");
            }   


            if (ModelState.IsValid)
            {
                _context.Add(sighting);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["BirdId"] = new SelectList(_context.Birds, "Id", "CommonName", sighting.BirdId);
            ViewData["SduserId"] = new SelectList(_context.Sdusers, "Id", "Id", sighting.SduserId);

                  
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

        // GET: BirdLog/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sighting = await _context.Sightings.FindAsync(id);
            if (sighting == null)
            {
                return NotFound();
            }
            ViewBag.SelectedLatLong = sighting.Latitude.HasValue && sighting.Longitude.HasValue
                ? $"{sighting.Latitude},{sighting.Longitude}"
                : null;
           
               
            
            ViewData["BirdId"] = new SelectList(_context.Birds, "Id", "CommonName", sighting.BirdId);
            ViewData["SduserId"] = new SelectList(_context.Sdusers, "Id", "Id", sighting.SduserId);

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
        public async Task<IActionResult> Edit(int id, [Bind("Id,SduserId,BirdId,Date,Latitude,Longitude,Notes")] Sighting sighting)
        {
            if (id != sighting.Id)
            {
                return NotFound();
            }
            
            var selectedLocation = Request.Form["PnwLocation"];

            if (sighting.BirdId == null || sighting.BirdId == 0)
            {
                if (sighting.BirdId == 0) // this means that "N/A" was selected
                {
                    sighting.BirdId = null;
                }
                else
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

            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sighting);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["BirdId"] = new SelectList(_context.Birds, "Id", "CommonName", sighting.BirdId);
            ViewData["SduserId"] = new SelectList(_context.Sdusers, "Id", "Id", sighting.SduserId);

                  
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
                .Include(s => s.Sduser)
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
    }
}
