// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Code for Map div
// Create map centered using the given default location
const map = L.map('map').setView([44.8485, -123.2340], 14);

// Add tile layer
const tiles = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
	maxZoom: 19,
	attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);


// fatch sightings from server
fetch('/api/map/GetSightings')
    .then(response => response.json())
    .then(data => {
        console.log("Fetched Sightings:", data); // Debugging: Ensure data is correct
        
        data.forEach(sighting => {
            console.log("Sighting Data:", sighting); // Check each sighting object
            
            if (sighting.latitude && sighting.longitude) { // Ensure coordinates exist
                L.marker([sighting.latitude, sighting.longitude])
                .addTo(map)
                .bindPopup(`<b>${sighting.commonName || 'Unknown Bird'}</b><br>
                            <em>${sighting.sciName || 'Unknown'}</em><br>
                            ${sighting.date ? new Date(sighting.date).toLocaleDateString() : "Unknown Date"}<br>
                            ${sighting.description || 'No notes available'}`);
            }
        });
    })
    .catch(error => console.error('Error fetching bird sightings:', error));
