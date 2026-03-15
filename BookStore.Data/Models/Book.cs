using BookStore.Data.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Data.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Book title is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1-200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "ISBN is required")]
        [StringLength(13, MinimumLength = 10, ErrorMessage = "ISBN must be between 10-13 characters")]
        [RegularExpression(@"^(?=(?:\D*\d){10,13}$)[\d-]+$", ErrorMessage = "Please enter a valid ISBN")]
        public string ISBN { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Author name must be between 2-100 characters")]
        public string Author { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10-2000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 10000, ErrorMessage = "Price must be between $0.01 and $10,000")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Cover image is required")]
        [Display(Name = "Cover Image")]
        public string CoverImageUrl { get; set; }

        [Range(1000, 2026, ErrorMessage = "Please enter a valid publication year")]
        [Display(Name = "Publication Year")]
        public int? PublicationYear { get; set; }

        [StringLength(100)]
        public string? Publisher { get; set; }

        [Range(1, 10000, ErrorMessage = "Page count must be between 1-10,000")]
        [Display(Name = "Page Count")]
        public int? PageCount { get; set; }

        [Required]
        public string UploadedById { get; set; }

        [Display(Name = "Added Date")]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(UploadedById))]
        public ApplicationUser UploadedBy { get; set; }

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Genre> Genres { get; set; } = new List<Genre>();
        public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();

        // Calculated properties
        [NotMapped]
        public double AverageRating => Ratings != null && Ratings.Any()
            ? Ratings.Average(r => r.Value)
            : 0;

        [NotMapped]
        public int RatingCount => Ratings?.Count ?? 0;

        [NotMapped]
        public int ReviewCount => Reviews?.Count ?? 0;
    }
}