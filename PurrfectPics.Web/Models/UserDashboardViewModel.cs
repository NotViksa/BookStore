using BookStore.Data.Models;
using BookStore.Data.Models.Identity;

namespace BookStore.Web.Models
{
    public class UserDashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<Book> UploadedImages { get; set; }

        public IEnumerable<Book> FavoriteImages { get; set; }
    }
}