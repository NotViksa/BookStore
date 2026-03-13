using BookStore.Data.Models.Identity;

namespace BookStore.Data.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public int CatImageId { get; set; }
        public Book CatImage { get; set; }

        public string PostedById { get; set; }
        public ApplicationUser PostedBy { get; set; }
    }
}