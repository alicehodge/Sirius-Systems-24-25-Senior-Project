// wwwroot/js/birdlog/create.js

function initializeBirdLogForm() {
    const form = document.getElementById('sightingForm');
    if (!form) return;

    // Image upload handling
    const photoFile = document.getElementById('photoFile');
    const photoPreviewContainer = document.getElementById('photoPreviewContainer');
    const photoPreview = document.getElementById('photoPreview');

    if (photoFile && photoPreviewContainer && photoPreview) {
        photoFile.addEventListener('change', function(e) {
            const file = e.target.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function(e) {
                    photoPreview.src = e.target.result;
                    photoPreviewContainer.style.display = 'block';
                }
                reader.readAsDataURL(file);
            }
        });
    }

    // Map functionality
    let createMap, createMarker;

    function initCreateMap() {
        try {
            const initialLat = document.getElementById('LatitudeInput').value || 44.8485;
            const initialLng = document.getElementById('LongitudeInput').value || -123.2340;
            
            createMap = L.map('map').setView([initialLat, initialLng], 14);

            L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                maxZoom: 19,
                attribution: '© OpenStreetMap contributors'
            }).addTo(createMap);

            createMap.on('click', function(e) {
                updateLocation(e.latlng);
            });

            if (initialLat && initialLng) {
                const initialCoords = L.latLng(initialLat, initialLng);
                updateLocation(initialCoords);
            }

        } catch (error) {
            console.error('Map initialization error:', error);
            const mapDiv = document.getElementById('map');
            if (mapDiv) {
                mapDiv.innerHTML = '<div class="alert alert-danger">Error loading map. Please refresh the page.</div>';
            }
        }
    }

    function updateLocation(latlng, displayName) {
        if (createMarker) createMap.removeLayer(createMarker);
        createMarker = L.marker(latlng).addTo(createMap);
        
        document.getElementById('LatitudeInput').value = latlng.lat;
        document.getElementById('LongitudeInput').value = latlng.lng;
        
        const locationSearch = document.getElementById('locationSearch');
        if (locationSearch) {
            if (displayName) {
                locationSearch.value = displayName;
            } else {
                fetch(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${latlng.lat}&lon=${latlng.lng}`, {
                    headers: { 'User-Agent': 'StorkDork/1.0 (contact@yourdomain.com)' }
                })
                .then(response => response.json())
                .then(data => {
                    locationSearch.value = data.display_name || 'Selected location';
                })
                .catch(error => {
                    console.error('Reverse geocode error:', error);
                    locationSearch.value = 'Selected location';
                });
            }
        }

        document.getElementById('latDisplay').textContent = latlng.lat.toFixed(4);
        document.getElementById('lngDisplay').textContent = latlng.lng.toFixed(4);
        document.getElementById('locationDisplay').style.display = 'block';
    }

    // Location search functionality
    const locationSearch = document.getElementById('locationSearch');
    const locationResults = document.getElementById('locationSearchResults');
    let searchTimeout;

    if (locationSearch && locationResults) {
        locationSearch.addEventListener('input', function(e) {
            const query = e.target.value.trim();
            clearTimeout(searchTimeout);
            
            if (query.length < 3) {
                locationResults.style.display = 'none';
                return;
            }

            searchTimeout = setTimeout(async () => {
                try {
                    const response = await fetch(
                        `https://nominatim.openstreetmap.org/search?q=${encodeURIComponent(query)}&format=json&limit=5`,
                        { headers: { 'User-Agent': 'StorkDork/1.0 (contact@yourdomain.com)' } }
                    );
                    
                    const results = await response.json();
                    locationResults.innerHTML = '';
                    
                    if (results.length === 0) {
                        locationResults.innerHTML = '<div class="location-result">No results found</div>';
                        locationResults.style.display = 'block';
                        return;
                    }
                    
                    results.forEach(result => {
                        const div = document.createElement('div');
                        div.className = 'location-result';
                        div.innerHTML = `
                            <div class="fw-bold">${result.display_name}</div>
                            <small>Lat: ${result.lat}, Lng: ${result.lon}</small>
                        `;
                        div.addEventListener('click', () => {
                            const latLng = L.latLng(result.lat, result.lon);
                            createMap.setView(latLng, 14);
                            updateLocation(latLng, result.display_name);
                            locationResults.style.display = 'none';
                        });
                        locationResults.appendChild(div);
                    });
                    
                    locationResults.style.display = 'block';
                } catch (error) {
                    console.error('Location search error:', error);
                    locationResults.innerHTML = '<div class="location-result">Error fetching results</div>';
                    locationResults.style.display = 'block';
                }
            }, 500);
        });
    }

    // Clear location handler
    const clearLocation = document.getElementById('clearLocation');
    if (clearLocation) {
        clearLocation.addEventListener('click', function() {
            if (createMarker) createMap.removeLayer(createMarker);
            document.getElementById('LatitudeInput').value = '';
            document.getElementById('LongitudeInput').value = '';
            document.getElementById('locationSearch').value = '';
            document.getElementById('locationDisplay').style.display = 'none';
        });
    }

    // Initialize the map
    initCreateMap();

    // Original form validation
    form.addEventListener('submit', function (event) {
        document.getElementById('birdError').style.display = 'none';
        document.getElementById('locationError').style.display = 'none';

        const birdId = document.getElementById('BirdId').value;
        const latitude = document.getElementById('LatitudeInput').value;
        const longitude = document.getElementById('LongitudeInput').value;
        let isValid = true;

        if (!birdId) {
            isValid = false;
            const birdError = document.getElementById('birdError');
            birdError.textContent = 'Please select a bird species';
            birdError.style.display = 'block';
        }

        if (!latitude || !longitude) {
            isValid = false;
            document.getElementById('locationError').textContent = 'Please select a location on the map';
            document.getElementById('locationError').style.display = 'block';
        }

        if (!isValid) {
            event.preventDefault();
            scrollToFirstError();
            return false;
        }

        return isValid;
    });

    // Bird search functionality
    const birdSearchInput = document.getElementById('birdSearch');
    const birdResultsContainer = document.getElementById('birdResults');

    if (birdSearchInput && birdResultsContainer) {
        birdSearchInput.addEventListener('input', function () {
            const searchTerm = this.value.trim();

            if (searchTerm.toLowerCase() === "n/a") {
                birdResultsContainer.innerHTML = '';
                const naOption = document.createElement('div');
                naOption.className = 'bird-result';
                naOption.innerHTML = `<strong>N/A</strong>`;
                naOption.addEventListener('click', function () {
                    birdSearchInput.value = "N/A";
                    document.getElementById('BirdId').value = "";
                    birdResultsContainer.style.display = 'none';
                });
                birdResultsContainer.appendChild(naOption);
                birdResultsContainer.style.display = 'block';
            } else if (searchTerm.length >= 2) {
                fetch(`/birds/search?term=${encodeURIComponent(searchTerm)}`)
                    .then(response => {
                        if (!response.ok) throw new Error('Network error');
                        return response.json();
                    })
                    .then(data => {
                        birdResultsContainer.innerHTML = '';
                        data.slice(0, 10).forEach(bird => {
                            const resultItem = document.createElement('div');
                            resultItem.className = 'bird-result';
                            resultItem.innerHTML = `
                                <strong>${bird.text}</strong>
                                <small>${bird.scientificName}</small>
                            `;
                            resultItem.addEventListener('click', function () {
                                birdSearchInput.value = bird.text;
                                document.getElementById('BirdId').value = bird.id;
                                birdResultsContainer.style.display = 'none';
                            });
                            birdResultsContainer.appendChild(resultItem);
                        });
                        birdResultsContainer.style.display = data.length > 0 ? 'block' : 'none';
                    })
                    .catch(error => {
                        console.error('Error fetching birds:', error);
                        birdResultsContainer.style.display = 'none';
                    });
            } else {
                birdResultsContainer.style.display = 'none';
            }
        });

        document.addEventListener('click', function (event) {
            if (!birdSearchInput.contains(event.target) && !birdResultsContainer.contains(event.target)) {
                birdResultsContainer.style.display = 'none';
            }
        });
    }
}

function scrollToFirstError() {
    const errors = document.querySelectorAll('.text-danger');
    const navbar = document.querySelector('.navbar');
    const navbarHeight = navbar ? navbar.offsetHeight : 0;
    
    const firstVisibleError = Array.from(errors).find(error => 
        window.getComputedStyle(error).display === 'block'
    );

    if (firstVisibleError) {
        const errorPosition = firstVisibleError.getBoundingClientRect().top + window.pageYOffset;
        const offsetPosition = errorPosition - navbarHeight - 20;
        window.scrollTo({ top: offsetPosition, behavior: 'smooth' });
    }
}

document.addEventListener('DOMContentLoaded', initializeBirdLogForm);