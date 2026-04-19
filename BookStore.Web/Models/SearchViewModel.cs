using BookStore.Data.Models;

namespace BookStore.Web.ViewModels
{
    public class SearchViewModel
    {
        public string SearchTerm { get; set; }

        public IEnumerable<Book> Results { get; set; }

        public int ResultCount { get; set; }

        // Filters
        public string? Genre { get; set; }

        public string? Author { get; set; }

        public decimal? MinPrice { get; set; }

        public decimal? MaxPrice { get; set; }

        // Sorting
        public string SortBy { get; set; } = "relevance";
    }
}