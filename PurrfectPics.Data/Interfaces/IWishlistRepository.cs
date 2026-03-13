using PurrfectPics.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Interfaces
{
    public interface IWishlistRepository
    {
        Task<Wishlist> GetFavoriteAsync(string userId, int imageId);
        Task AddFavoriteAsync(Wishlist favorite);
        Task RemoveFavoriteAsync(Wishlist favorite);
        Task<bool> FavoriteExistsAsync(string userId, int imageId);
        Task<IEnumerable<Book>> GetUserFavoriteImagesAsync(string userId);
        Task<int> GetFavoriteCountByUserAsync(string userId);
        Task<IEnumerable<Wishlist>> GetRecentFavoritesAsync(string userId, int count);
    }
}