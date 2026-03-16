using BookStore.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace BookStore.Web.Models
{
    public class BookDetailsViewModel
    {
        public Book Book { get; set; }

        public bool IsInWishlist { get; set; }

        [Range(1, 5)]
        public int? UserRating { get; set; }

        public double AverageRating { get; set; }

        public IEnumerable<Review> Reviews { get; set; }

        public IEnumerable<Book> SimilarBooks { get; set; }

        // Stats
        public int TotalReviews => Reviews?.Count() ?? 0;

        public Dictionary<int, int> RatingDistribution { get; set; }
    }
}