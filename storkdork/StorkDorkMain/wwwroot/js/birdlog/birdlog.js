

// wwwroot/js/birdlog/birdlog.js

//CREATE.CSHTML JAVASCRIPT
// Function to initialize the form validation and event listeners
function initializeBirdLogForm() {
    const form = document.getElementById('sightingForm');
    if (!form) return; // Exit if the form doesn't exist on the page

    // Add event listener for form submission
    form.addEventListener('submit', function (event) {
        // Get the selected bird and location values
        const birdId = document.getElementById('BirdId').value;
        const location = document.getElementById('PnwLocation').value;

        // Get the error message boxes
        const birdError = document.getElementById('birdError');
        const locationError = document.getElementById('locationError');

        // Reset any previous error messages
        birdError.textContent = "";
        locationError.textContent = "";

        // Assume the form is valid unless we find a problem
        let isValid = true;

        // Check if both bird and location are left blank (not even N/A)
        if (birdId === '' && location === '') {
            // If both are left blank, show an error
            event.preventDefault(); // Stop the form from submitting
            birdError.textContent = "Please select a bird or choose N/A."; // Show error message for bird
            locationError.textContent = "Please select a location or choose N/A."; // Show error message for location
            isValid = false; // Mark the form as invalid

            // Scroll to the first error message
            scrollToFirstError();
        } else if (birdId === '') {
            // If bird is left blank (not even N/A), show an error
            event.preventDefault();
            birdError.textContent = "Please select a bird or choose N/A.";
            isValid = false;

            // Scroll to the bird error message
            birdError.scrollIntoView({ behavior: 'smooth', block: 'center' });
        } else if (location === '') {
            // If location is left blank (not even N/A), show an error
            event.preventDefault();
            locationError.textContent = "Please select a location or choose N/A.";
            isValid = false;

            // Scroll to the location error message
            locationError.scrollIntoView({ behavior: 'smooth', block: 'center' });
        }

        // If the form is valid, it will submit. If not, it will stop and show the error.
        return isValid;
    });

    // Add event listener for location dropdown change
    const locationDropdown = document.getElementById('PnwLocation');
    if (locationDropdown) {
        locationDropdown.addEventListener('change', function () {
            // Get the selected option from the dropdown
            const selectedOption = this.options[this.selectedIndex];

            // Get the latitude and longitude from the selected option
            const latLong = selectedOption.getAttribute('data-latlong');

            // If latitude and longitude are available, split them and update the hidden fields
            if (latLong) {
                const [latitude, longitude] = latLong.split(','); // Split the string into two parts
                document.getElementById('LatitudeInput').value = latitude; // Update latitude field
                document.getElementById('LongitudeInput').value = longitude; // Update longitude field
            } else {
                // If no latitude and longitude are available, clear the hidden fields
                document.getElementById('LatitudeInput').value = '';
                document.getElementById('LongitudeInput').value = '';
            }
        });
    }
}

// Function to scroll to the first error message
function scrollToFirstError() {
    const firstError = document.querySelector('.text-danger');
    if (firstError) {
        firstError.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
}

// Initialize the form when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', initializeBirdLogForm);

///////////////




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