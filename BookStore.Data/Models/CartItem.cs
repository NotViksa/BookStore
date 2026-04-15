using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookStore.Data.Models.Identity;

namespace BookStore.Data.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, 99)]
        public int Quantity { get; set; } = 1;

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }
    }
}