// Code for Map div
// Create map centered using the given default location
const map = L.map('map').setView([44.8485, -123.2340], 14);

// Add tile layer
const tiles = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
	maxZoom: 19,
	attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

// function fatchSightingsByUser()

// Function to fetch and display sightings
function fetchSightings(userId) {
    let url = '/api/map/GetSightings'; // Default: all sightings
    if (userId && userId !== 'all') {
        url = `/api/map/GetSightings/${userId}`; // Fetch specific user's sightings
    }

    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }
            return response.json();
        })
        .then(data => {
            console.log("Fetched Sightings:", data); // Debugging

            // Clear existing markers
            map.eachLayer((layer) => {
                if (layer instanceof L.Marker) {
                    map.removeLayer(layer);
                }
            });

            // Check if data is received
            if (!data || data.length === 0) {
                console.warn("No sightings found.");
                return;
            }

            data.forEach(sighting => {
                console.log("Sighting Data:", sighting); // Debugging

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
}

// Event listener for user selection
document.getElementById('userSelect').addEventListener('change', (event) => {
    fetchSightings(event.target.value);
});