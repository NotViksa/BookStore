using BookStore.Data.Models;

namespace BookStore.Web.ViewModels
{
    public class GenreDetailsViewModel
    {
        public Genre Genre { get; set; }

        public IEnumerable<Book> Books { get; set; }

        public int BookCount { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}