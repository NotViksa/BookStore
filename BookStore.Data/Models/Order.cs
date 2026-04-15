using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BookStore.Data.Models.Identity;

namespace BookStore.Data.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }

        [StringLength(200)]
        public string ShippingAddress { get; set; }

        [StringLength(50)]
        public string PaymentMethod { get; set; } = "Fake Payment";

        [StringLength(20)]
        public string OrderStatus { get; set; } = "Pending";

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}