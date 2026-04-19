using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface IRatingService
    {
        // Rating operations
        Task<(bool Success, double AverageRating)> SubmitRatingAsync(string userId, int bookId, int ratingValue);
        Task<bool> RemoveRatingAsync(string userId, int bookId);

        // Queries
        Task<double> GetAverageRatingForBookAsync(int bookId);
        Task<int?> GetUserRatingAsync(string userId, int bookId);
        Task<Rating?> GetRatingAsync(string userId, int bookId);

        // Statistics
        Task<Dictionary<int, int>> GetRatingDistributionAsync(int bookId);
        Task<int> GetTotalRatingsCountAsync(int bookId);
    }
}