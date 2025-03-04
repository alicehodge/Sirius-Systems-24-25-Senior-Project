using System.Text.Json.Serialization;

namespace StorkDorkMain.Models;
public class EBirdSighting
{
    [JsonPropertyName("speciesCode")]
    public string SpeciesCode { get; set; }

    [JsonPropertyName("comName")]
    public string CommonName { get; set; }

    [JsonPropertyName("sciName")]
    public string ScientificName { get; set; }

    [JsonPropertyName("locId")]
    public string LocationId { get; set; }

    [JsonPropertyName("locName")]
    public string LocationName { get; set; }

    [JsonPropertyName("obsDt")]
    public string ObservationDate { get; set; }

    [JsonPropertyName("howMany")]
    public int? Count { get; set; }

    [JsonPropertyName("lat")]
    public double Latitude { get; set; }

    [JsonPropertyName("lng")]
    public double Longitude { get; set; }
}