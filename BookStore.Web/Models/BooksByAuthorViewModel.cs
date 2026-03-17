using BookStore.Data.Models;

namespace BookStore.Web.ViewModels
{
    public class BooksByAuthorViewModel
    {
        public string Author { get; set; }

        public IEnumerable<Book> Books { get; set; }

        public int BookCount { get; set; }

        public IEnumerable<string> Genres { get; set; }
    }
}