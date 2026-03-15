using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface ICommentService
    {
        Task<int> GetCommentCountByUserAsync(string userId);
        Task<Review> AddCommentAsync(string content, int catImageId, string userId);
        Task<IEnumerable<Review>> GetCommentsForImageAsync(int catImageId);
        Task<bool> DeleteCommentAsync(int commentId, string userId, bool isAdmin);
        Task<bool> EditCommentAsync(int commentId, string? userId, string content);
    }
}