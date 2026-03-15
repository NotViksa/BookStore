using BookStore.Data.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Data.Models
{
    public class Wishlist
    {
        public int Id { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "Added Date")]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

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