// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Code for Map div
// Create map centered using the given default location
const map = L.map('map').setView([51.505, -0.09], 13);

// Add tile layer
const tiles = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
	maxZoom: 19,
	attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);


// fatch sightings from server
fetch('/Leaflet/GetSightings')
	.then(response => response.json())
	.then(data => {
		// add markers for each sighting
		data.forEach(sighting => {
			L.marker([sighting.latitude, sighting.longitude])
			.addTo(map)
			.bindPopup(`<b>${sighting.CommonName}</b><br>${sighting.Date}<br>${sighting.Notes}`);
		});
	})
	.catch(error => console.log('Error fetching bird sightings data: ', error));

