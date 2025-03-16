const geoJson = require ('world-geojson');

const globalGeoJson = geoJson.combineGeoJson([
    {countryname: 'Antigua & Barbuda'},
    {countryName : 'Australia', stateName: 'New South Wales'},
    {countryName: 'U.S.A.', areaName: 'U.S. Virgin Islands'}
]);