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
        public string? Bio { get; set; }

        public string ProfileImageUrl { get; set; } = "/images/default-profile.png";

        // New fields for book store
        [StringLength(50)]
        public string? FavoriteGenre { get; set; }

        [Range(1900, 2026, ErrorMessage = "Please enter a valid birth year")]
        public int? BirthYear { get; set; }
        public ICollection<Book> UploadedBooks { get; set; } = new List<Book>();
        public ICollection<Wishlist> Wishlist { get; set; } = new List<Wishlist>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}