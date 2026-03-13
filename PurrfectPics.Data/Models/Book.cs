using Azure;
using PurrfectPics.Data.Models.Identity;
using System.ComponentModel.DataAnnotations;

namespace PurrfectPics.Data.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        public string Description { get; set; }

        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        [Required]
        public string UploadedById { get; set; }
        public ApplicationUser UploadedBy { get; set; }

        public ICollection<Review> Comments { get; set; } = new List<Review>();
        public ICollection<Genre> Tags { get; set; } = new List<Genre>();
        public ICollection<Wishlist> Favorites { get; set; } = new List<Wishlist>();
        public ICollection<Rating> Votes { get; set; } = new List<Rating>();
    }
}