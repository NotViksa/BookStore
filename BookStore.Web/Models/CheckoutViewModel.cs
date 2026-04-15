using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.ViewModels
{
    public class CheckoutViewModel
    {
        [Required]
        [StringLength(200)]
        [Display(Name = "Shipping Address")]
        public string ShippingAddress { get; set; }    }
}