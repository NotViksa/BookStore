using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookStore.Data.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "Display name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Display name must be between 2-50 characters")]
        public string DisplayName { get; set; } = "New User";

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [StringLength(500, ErrorMessage = "Bio cannot exceed 500 characters")]
        public string? ProfileBio { get; set; }

        public string ProfileImageUrl { get; set; } = "/images/default-profile.png";

        // Navigation properties
        public ICollection<Book> UploadedImages { get; set; } = new List<Book>();
        public ICollection<Wishlist> Favorites { get; set; } = new List<Wishlist>();
        public ICollection<Rating> Votes { get; set; } = new List<Rating>();
        public ICollection<Review> Comments { get; set; } = new List<Review>();

    }
}