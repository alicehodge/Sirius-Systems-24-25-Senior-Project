const { makeSightingMarkers } = require("../../StorkDorkMain/wwwroot/js/map/map.js");

// Setup for tests

jest.mock("../../StorkDorkMain/wwwroot/js/map/map.js", () => ({
    ...jest.requireActual("../../StorkDorkMain/wwwroot/js/map/map.js"),
    fetchSightingsByUser: jest.fn(),
    makeSightingMarkers: jest.fn(),
}));

global.fetch = jest.fn();

// Tests for fetchSightingsByUser

// Tests for makeSightingMarkers

describe("makeSightingMarkers", () => {
    beforeEach(() => {
        L.marker.mockClear();
    });

    test("should not create markers if coordinates are missing", () => {
        const sightings = [{ commonName: "Sparrow", sciName: "Passer domesticus" }];

        makeSightingMarkers(sightings);

        expect(L.marker).not.toHaveBeenCalled();
    });
});