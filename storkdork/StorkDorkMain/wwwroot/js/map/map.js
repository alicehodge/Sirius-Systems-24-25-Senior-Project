// Written by Christian Raymon

// module.exports = {fetchUser, fetchSightingsByUser, makeSightingMarkers};

// Code for Map div
// Create map centered using the given default location
let map = L.map('map').setView([44.8485, -123.2340], 14);

const allSightingsGroup = L.layerGroup().addTo(map);
const usersSightingsGroup = L.layerGroup().addTo(map);

const overlayMaps ={
    "All Sightings": allSightingsGroup,
    "My Sightings": usersSightingsGroup,
};

// Add tile layer
const tiles = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
	maxZoom: 19,
	attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);

let markers = {};

var storkDorkIcon = L.icon({
    iconUrl: '../images/map/StorkMarkerIcon.png',
    iconSize: [64, 64]
})


/* geolocation courtesy of Alice Hodge */
// document.addEventListener('DOMContentLoaded', function() {
//     setupGeolocation();
// });

function setupGeolocation() {
    if ("geolocation" in navigator) {
        navigator.geolocation.getCurrentPosition(
            function(position) {
                map.setView([position.coords.latitude, position.coords.longitude], 13);
            },
            function(error) {
                console.error("Geolocation error:", error.message);
            }
        );
    }
}

/* --------------------------------- */

async function fetchAllSightings() {
    let url = `/api/map/GetSightings`;
    let response = await fetch(url);

    if (!response.ok)
    {
        console.error("failed to fetch sightings:", response.statusText);
        return;
    }

    let sightings = await response.json();

    allSightingsGroup.clearLayers();
    usersSightingsGroup.clearLayers();

    makeSightingMarkers(sightings);
}

async function fetchSightingsByUser(user) {
    let url = `/api/map/GetSightings/${user.id}`;
    let response = await fetch(url);

    if (!response.ok) {
        console.error("Failed to fetch sightings:", response.statusText);
        return;
    }

    let sightings = await response.json();

    allSightingsGroup.clearLayers();
    usersSightingsGroup.clearLayers();

    makeSightingMarkers(sightings, user.id);
}

async function fetchOtherSightings(userId) {
    let url = `/api/map/GetOtherSightings/${userId}`;
    let response = await fetch(url);

    if (!response.ok) {
        console.error("Failed to fetch other sightings:", response.statusText);
        return;
    }

    let sightings = await response.json();
    allSightingsGroup.clearLayers();
    makeSightingMarkers(sightings, null);
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

async function reverseGeocode(lat, lng) {
    const url = `https://nominatim.openstreetmap.org/reverse?lat=${lat}&lon=${lng}&format=json`;

    try {
        const response = await fetch(url);
        const data = await response.json();

        if (data && data.address) {
            const country = data.address.country || "Unknown Country";
            const subdivision = data.address.state || data.address.province || "Unknown Region";

            return `${subdivision}, ${country}`;
        }
    } catch (error) {
        console.error("Reverse geocoding failed:", error);
    }
    return "Location not found";
}

async function makeSightingMarkers(data, userId = null) {
    data.forEach(async sighting => {

        if (sighting.latitude && sighting.longitude) {
            const marker = L.marker([sighting.latitude, sighting.longitude], {icon: storkDorkIcon});

            allSightingsGroup.addLayer(marker);

            if (sighting.userId === userId)
                usersSightingsGroup.addLayer(marker);

            marker.addTo(map);

            // Store marker reference for later updates
            markers[sighting.sightingId] = marker;

            const popupContent = `<b>${sighting.commonName || 'Unknown Bird'}</b><br>
                                  <em>${sighting.sciName || 'Unknown'}</em><br>
                                  <strong>Spotted by: ${sighting.birder}</strong><br>
                                  ${sighting.date ? new Date(sighting.date).toLocaleDateString() : "Unknown Date"}<br>
                                  ${sighting.description || 'No notes available'}<br>
                                  <strong>Location:</strong> ${sighting.subdivision || 'Unknown'}, ${sighting.country || 'Unknown'}`;

            marker.bindPopup(popupContent);

            if (!sighting.country || !sighting.subdivision) {
                const location = await reverseGeocode(sighting.latitude, sighting.longitude);

                if (location !== "location not found") {
                    const [subdivision, country] = location.split(',');

                    // Update backend
                    await updateSightingLocation(sighting.sightingId, country.trim(), subdivision.trim());

                    // Update marker popup dynamically
                    marker.setPopupContent(`<b>${sighting.commonName || 'Unknown Bird'}</b><br>
                                            <em>${sighting.sciName || 'Unknown'}</em><br>
                                            <strong>Spotted by: ${sighting.birder}</strong><br>
                                            ${sighting.date ? new Date(sighting.date).toLocaleDateString() : "Unknown Date"}<br>
                                            ${sighting.description || 'No notes available'}<br>
                                            <strong>Location:</strong> ${subdivision.trim()}, ${country.trim()}`);

                }
            }
        }
    });
}

async function updateSightingLocation(sightingId, country, subdivision) {
    const payload = {
        sightingId: sightingId, 
        country: country || "Unknown", 
        subdivision: subdivision || "Unknown"
    };

    // console.log("Sending payload:", payload);

    try {
        const response = await fetch("/api/sighting/UpdateLocation", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            // const errorText = await response.text();
            // throw new Error(`Failed to update location: ${response.status} - ${errorText}`);
        }

        // console.log(`Successfully updated location for sighting ${sightingId}`);
    } catch (error) {
        // console.error("Error updating sighting location:", error);
    }
}

async function checkUserLoggedIn() {
    try {
        const response = await fetch("/api/User/is-user-logged-in");

        if (!response.ok) {
            console.warn("Login status check failed:", response.statusText);
            return false;
        }

        return await response.json();
    } catch (error) {
        console.error("Login check failed:", error);
        return false;
    }
}

window.onload = async function() {
    setupGeolocation();

    const isLoggedIn = await checkUserLoggedIn();

    if (isLoggedIn) {
        const user = await fetchUser(); // still get full info
        if (user && typeof user.id === "number") {
            await fetchSightingsByUser(user);
            await fetchOtherSightings(user.id);
        } else {
            console.warn("User check passed but couldn't fetch full user info.");
            await fetchAllSightings();
        }
    } else {
        await fetchAllSightings();
    }

    L.control.layers(null, overlayMaps).addTo(map);
}