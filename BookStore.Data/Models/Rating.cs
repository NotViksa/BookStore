using BookStore.Data.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Data.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Rating value is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        public int Value { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Rated Date")]
        public DateTime RatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        [Required]
        public string UserId { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
    }
}