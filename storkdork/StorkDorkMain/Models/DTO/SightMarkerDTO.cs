// Used for getting information on a sighting popup for the markers on the map page

namespace StorkDorkMain.Models.DTO;

public class SightMarker 
{
    public int SightingId {get;set;}
    public string? CommonName {get;set;}
    public string? SciName {get;set;}
    public decimal? Longitude {get;set;}
    public decimal? Latitude {get;set;}
    public string? Description {get;set;}
    public DateTime? Date {get;set;}
}