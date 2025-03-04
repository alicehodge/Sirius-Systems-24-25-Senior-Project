using System.Security.Permissions;

namespace StorkDorkMain.Models
{
    public class RelatedBirdViewModel
    {
        public Bird Bird { get; set; }
        public string RelationType { get; set; } // eBird taxonomic category
        
        public string GetRelationType()
        {
            return Bird.Category switch
            {
                "species" => "Species",
                "issf" => "Subspecies",
                "hybrid" => "Hybrid",
                "intergrade" => "Intergrade",
                "spuh" => "Species Group",
                "slash" => "Possible Species",
                "domestic" => "Domestic Form",
                "form" => "Regional Form",
                _ => "Other"
            };
        }
    }

    public class BirdDetailsViewModel
    {
        public Bird Bird { get; set; }
        public string ImageUrl { get; set; } = "/images/placeholder-bird.svg";
        public List<RelatedBirdViewModel> RelatedBirds { get; set; } = new List<RelatedBirdViewModel>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string? CategoryFilter { get; set; }
        
        public static readonly List<string> Categories = new()
        {
            "species",
            "issf",
            "hybrid",
            "intergrade",
            "spuh",
            "slash",
            "domestic",
            "form"
        };
    }
}