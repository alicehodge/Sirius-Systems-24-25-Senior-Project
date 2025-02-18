using System.Security.Permissions;

namespace StorkDorkMain.Models
{
    public class BirdDetailsViewModel
    {
        public string CommonName { get; set; }
        public string ScientificName { get; set; }
        public string Order { get; set; }
        public string FamilyCommonName { get; set; }
        public string FamilyScientificName { get; set; }
        public string Range { get; set; } = "Unknown";
        public string ImageUrl { get; set; } = "/images/placeholder-bird.svg";
    }
}