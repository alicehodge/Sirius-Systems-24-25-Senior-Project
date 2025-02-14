// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Code for Map div
const map = L.map('map').setView([51.505, -0.09], 13);

const tiles = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
	maxZoom: 19,
	attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">OpenStreetMap</a>'
}).addTo(map);






// sightingForm.js





document.addEventListener('DOMContentLoaded', function () {
    var latInput = document.getElementById('LatitudeInput');
    var longInput = document.getElementById('LongitudeInput');
    var locationDropdown = document.getElementById('PnwLocation');

    // Get the selected location from the ViewBag
    var selectedLatLong = '@ViewBag.SelectedLatLong';

    console.log(`Selected Location: ${selectedLatLong}`); // Debugging

    // Pre-select the correct location based on the selectedLatLong value
    for (var i = 0; i < locationDropdown.options.length; i++) {
        var option = locationDropdown.options[i];
        if (option.value === selectedLatLong) {
            locationDropdown.selectedIndex = i;
            console.log(`Selected Option: ${option.text}`);
            break;
        }
    }

    // Update hidden fields when a new location is selected
    locationDropdown.addEventListener('change', function () {
        var selectedOption = this.options[this.selectedIndex];
        var latLong = selectedOption.value.split(',');

        if (latLong.length === 2) {
            latInput.value = latLong[0];
            longInput.value = latLong[1];
        } else {
            latInput.value = "";
            longInput.value = "";
        }

        console.log(`Updated Latitude: ${latInput.value}, Longitude: ${longInput.value}`); // Debugging
    });
});