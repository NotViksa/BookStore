using BookStore.Data.Models;

namespace BookStore.Web.ViewModels
{
    public class WishlistViewModel
    {
        public IEnumerable<Book> Books { get; set; }

        public int TotalItems { get; set; }

        public decimal TotalValue => Books?.Sum(b => b.Price) ?? 0;

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}