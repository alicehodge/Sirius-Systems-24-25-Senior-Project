namespace StorkDorkMain.Models
{
    public class SearchResultsViewModel
    {
        public IEnumerable<Bird> Birds { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SearchTerm { get; set; }
        public string SearchType { get; set; } = "name";
    }
}