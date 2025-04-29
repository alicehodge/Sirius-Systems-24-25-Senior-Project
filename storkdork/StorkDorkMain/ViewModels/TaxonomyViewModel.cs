namespace StorkDorkMain.Models
{
    public class TaxonomyViewModel
    {
        public IEnumerable<string> Orders { get; set; }
        public IEnumerable<(string ScientificName, string CommonName)> Families { get; set; }
    }

    public class TaxonomyListViewModel
    {
        public string Title { get; set; }
        public IEnumerable<Bird> Birds { get; set; }
        public string CurrentSort { get; set; }
        public string TaxonomicGroup { get; set; }
    }
}