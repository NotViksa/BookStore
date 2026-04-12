using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface IReviewService
    {
        // Basic operations
        Task<Review> AddReviewAsync(string content, int rating, int bookId, string userId);
        Task<bool> DeleteReviewAsync(int reviewId, string userId, bool isAdmin);
        Task<bool> EditReviewAsync(int reviewId, string userId, string content, int rating);

        // Queries
        Task<Review?> GetReviewByIdAsync(int reviewId);
        Task<IEnumerable<Review>> GetReviewsForBookAsync(int bookId);
        Task<int> GetReviewCountByUserAsync(string userId);
        Task<double> GetAverageRatingForBookAsync(int bookId);
        IQueryable<Review> GetReviewsByUserQueryable(string userId);
        Task<IEnumerable<Review>> GetReviewsByUserAsync(string userId);
    }
}