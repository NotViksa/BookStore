using BookStore.Data.Models.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookStore.Data.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public DateTime FavoritedDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("CatImage")]
        public int CatImageId { get; set; }
        public Book CatImage { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}