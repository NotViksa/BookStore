using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface IWishlistService
    {
        // Wishlist management
        Task<bool> ToggleWishlistAsync(string userId, int bookId);
        Task<bool> IsInWishlistAsync(string userId, int bookId);

        // Queries
        Task<IEnumerable<Book>> GetUserWishlistAsync(string userId);
        Task<int> GetWishlistCountByUserAsync(string userId);
        Task<IEnumerable<Wishlist>> GetRecentWishlistItemsAsync(string userId, int count);

        // Bulk operations
        Task<bool> ClearWishlistAsync(string userId);
        Task<bool> RemoveFromWishlistAsync(string userId, int bookId);
        IQueryable<Book> GetUserWishlistQueryable(string userId);
    }
}