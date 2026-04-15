using BookStore.Data.Models;
using BookStore.Data.Models.Identity;

namespace BookStore.Web.Models
{
    public class ProfileViewModel
    {
        public ApplicationUser User { get; set; }

        public IEnumerable<Book> UploadedBooks { get; set; }

        public IEnumerable<Book> WishlistBooks { get; set; }

        public int UploadCount { get; set; }

        public int WishlistCount { get; set; }

        public int ReviewCount { get; set; }

        // Recent activity
        public IEnumerable<Review> RecentReviews { get; set; }

        public DateTime MemberSince => User?.RegistrationDate ?? DateTime.UtcNow;

        public string ProfileImageUrl => User?.ProfileImageUrl ?? "/images/default-profile.png";
        public int PurchasedCount { get; set; }
        public IEnumerable<Book> PurchasedBooks { get; set; } = new List<Book>();
    }
}