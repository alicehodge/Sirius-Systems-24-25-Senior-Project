// wwwroot/js/birdlog/birdlog.js



// wwwroot/js/birdlog/birdlog.js
// Function to initialize the form validation and event listeners for the Edit view
function initializeEditBirdLogForm() {
    const form = document.getElementById('sightingForm');
    if (!form) return; // Exit if the form doesn't exist on the page

    // Add event listener for form submission
    form.addEventListener('submit', function (event) {
        const birdId = document.getElementById('BirdId').value;
        const location = document.getElementById('PnwLocation').value;
        const locationError = document.getElementById('locationError'); // Error span for location
        let isValid = true;

        locationError.textContent = ""; // Reset error message

        // Ensure at least one selection is made (Bird or Location)
        if ((birdId === '' || birdId === '0') && (location === '' || location === '0')) {
            // Allow both to be N/A (0)
            // No need to prevent submission or show an error
        } else if (location === '' && location !== '0') {
            // If location is empty and not explicitly set to N/A, show an error
            event.preventDefault();
            locationError.textContent = "Please select a location or choose N/A.";
            isValid = false;

            // Scroll to the first error message
            scrollToFirstError();
        }

        return isValid; // Allow form submission if validation passes
    });

    // Ensure location selection updates hidden fields correctly
    const locationDropdown = document.getElementById('PnwLocation');
    if (locationDropdown) {
        locationDropdown.addEventListener('change', function () {
            const selectedOption = this.options[this.selectedIndex];
            const latLong = selectedOption.getAttribute('data-latlong');

            if (latLong) {
                const [latitude, longitude] = latLong.split(',');
                document.getElementById('LatitudeInput').value = latitude;
                document.getElementById('LongitudeInput').value = longitude;
            } else {
                document.getElementById('LatitudeInput').value = '';
                document.getElementById('LongitudeInput').value = '';
            }
        });
    }

    // Initialize the dropdown and hidden fields with the current values
    const selectedLatLong = "@ViewBag.SelectedLatLong"; // Get the selected location from ViewBag
    if (selectedLatLong) {
        // Find the option that matches the selected location
        for (let i = 0; i < locationDropdown.options.length; i++) {
            if (locationDropdown.options[i].value === selectedLatLong) {
                locationDropdown.selectedIndex = i; // Set the dropdown to the selected option
                break;
            }
        }

        // Update the hidden fields with the selected location's latitude and longitude
        const selectedOption = locationDropdown.options[locationDropdown.selectedIndex];
        const latLong = selectedOption.getAttribute('data-latlong');

        if (latLong) {
            const [latitude, longitude] = latLong.split(',');
            document.getElementById('LatitudeInput').value = latitude;
            document.getElementById('LongitudeInput').value = longitude;
        }
    }
}

// Function to scroll to the first error message
function scrollToFirstError() {
    const firstError = document.querySelector('.text-danger');
    if (firstError) {
        firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
}

// Initialize the Edit form when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', function () {
    initializeEditBirdLogForm();
});

