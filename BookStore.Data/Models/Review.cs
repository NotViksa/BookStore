using BookStore.Data.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Data.Models
{
    public class Review
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Review content is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Review must be between 10-2000 characters")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Rating { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Posted Date")]
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        [Required]
        public string PostedById { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }

        [ForeignKey(nameof(PostedById))]
        public ApplicationUser PostedBy { get; set; }
    }
}