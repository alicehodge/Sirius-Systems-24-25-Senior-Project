<<<<<<< HEAD
module.exports = {fetchUser, fetchSightingsByUser, makeSightingMarkers};
=======
//module.exports = {fetchUser, fetchSightingsByUser, makeSightingMarkers};
>>>>>>> dev

// Code for Map div
// Create map centered using the given default location
const map = L.map('map').setView([44.8485, -123.2340], 14);

// Add tile layer
const tiles = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
	maxZoom: 19,
	attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

async function fetchSightingsByUser() {
    let user = await fetchUser();

    if (!user || typeof user.id !== "number") {
        console.error("Invalid or missing ID: ", user);
        return;
    }

    let url = `/api/map/GetSightings/${user.id}`;
    let response = await fetch(url);

    if (!response.ok) {
        console.error("Failed to fetch sightings:", response.statusText);
        return;
    }

    let sightings = await response.json();
    console.log("Fetched sightings: ", sightings);

    map.eachLayer((layer) => {
        if(layer instanceof L.Marker){
            map.removeLayer(layer);
        }
    });

    makeSightingMarkers(sightings);
}

async function fetchUser() {
    let url = "/api/User/current-user";
    try {
        let response = await fetch(url);
        
        if (!response.ok) {
            let errorText = await response.text(); // Get error message
            throw new Error(`HTTP ${response.status}: ${errorText}`);
        }

        return await response.json();
    } catch (error) {
        console.error("Error fetching user:", error);
        return null;
    }
}

function makeSightingMarkers(data)
{
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
}

window.onload = function() {
    fetchSightingsByUser();
}