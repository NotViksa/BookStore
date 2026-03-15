using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Data.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Genre name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Genre name must be between 2-50 characters")]
        [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "Genre name can only contain letters, spaces, and hyphens")]
        public string Name { get; set; }

        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
        public string? Description { get; set; }

        // Navigation property
        public ICollection<Book> Books { get; set; } = new List<Book>();

        // Calculated property
        [NotMapped]
        public int BookCount => Books?.Count ?? 0;
    }
}