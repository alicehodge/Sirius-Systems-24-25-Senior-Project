// Used for getting information on a sighting popup for the markers on the map page

namespace StorkDorkMain.Models.DTO;

public class SightMarker 
{
    public string? CommonName {get;set;}
    public string? SciName {get;set;}
    public string? Description {get;set;}
    public DateTime? Date {get;set;}
}