using BookStore.Data.Models;

namespace BookStore.Web.Models
{
    public class CatImageDetailsViewModel
    {
        public Book Image { get; set; }
        public bool IsFavorited { get; set; }
        public bool? UserVote { get; set; } // true=upvote, false=downvote, null=no vote
        public int Score { get; set; }
        public Book CatImage { get; set; }
        public IEnumerable<Review> Comments { get; set; }
    }
}