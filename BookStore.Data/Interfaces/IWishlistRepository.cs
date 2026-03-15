using BookStore.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Data.Interfaces
{
    public interface IWishlistRepository
    {
        Task<Wishlist> GetFavoriteAsync(string userId, int bookId);
        Task AddFavoriteAsync(Wishlist wishlist);
        Task RemoveFavoriteAsync(Wishlist wishlist);
        Task<bool> FavoriteExistsAsync(string userId, int bookId);
        Task<IEnumerable<Book>> GetUserFavoriteImagesAsync(string userId);
        Task<int> GetFavoriteCountByUserAsync(string userId);
        Task<IEnumerable<Wishlist>> GetRecentFavoritesAsync(string userId, int count);
    }
}