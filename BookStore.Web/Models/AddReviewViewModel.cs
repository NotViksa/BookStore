using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.ViewModels
{
    public class AddReviewViewModel
    {
        [Required]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Please enter your review")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Review must be between 10-2000 characters")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Your Review")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Please select a rating")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5 stars")]
        [Display(Name = "Rating")]
        public int Rating { get; set; }
    }
}