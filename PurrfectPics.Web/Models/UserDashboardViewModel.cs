using PurrfectPics.Data.Models;
using PurrfectPics.Data.Models.Identity;

namespace PurrfectPics.Web.Models
{
    public class UserDashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<Book> UploadedImages { get; set; }

        public IEnumerable<Book> FavoriteImages { get; set; }
    }
}