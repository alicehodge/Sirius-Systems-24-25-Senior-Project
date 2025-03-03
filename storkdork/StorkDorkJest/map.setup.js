global.L = {
    map: jest.fn(() => ({
        setView: jest.fn().mockReturnThis(),
        eachLayer: jest.fn(),
        removeLayer: jest.fn(),
        addLayer: jest.fn(),
    })),
    tileLayer: jest.fn(() => ({
        addTo: jest.fn(),
    })),
    marker: jest.fn(() => ({
        addTo: jest.fn(),
        bindPopup: jest.fn(),
    })),
};
