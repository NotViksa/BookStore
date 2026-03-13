using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface IFavoriteService
    {
        Task<bool> ToggleFavoriteAsync(string userId, int imageId);
        Task<bool> IsFavoritedAsync(string userId, int imageId);
        Task<IEnumerable<Book>> GetUserFavoritesAsync(string userId);
        Task<int> GetFavoriteCountByUserAsync(string userId);
        Task<IEnumerable<Wishlist>> GetRecentFavoritesAsync(string userId, int count);
    }
}