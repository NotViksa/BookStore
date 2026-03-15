using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class FavoriteService : IWishlistService
    {
        private readonly IWishlistRepository _favoriteRepository;

        public FavoriteService(IWishlistRepository favoriteRepository)
        {
            _favoriteRepository = favoriteRepository;
        }

        public async Task<bool> ToggleFavoriteAsync(string userId, int imageId)
        {
            var existing = await _favoriteRepository.GetFavoriteAsync(userId, imageId);

            if (existing != null)
            {
                await _favoriteRepository.RemoveFavoriteAsync(existing);
                return false;
            }

            var favorite = new Wishlist
            {
                UserId = userId,
                CatImageId = imageId,
                FavoritedDate = System.DateTime.UtcNow
            };

            await _favoriteRepository.AddFavoriteAsync(favorite);
            return true;
        }

        public async Task<bool> IsFavoritedAsync(string userId, int imageId)
        {
            return await _favoriteRepository.FavoriteExistsAsync(userId, imageId);
        }

        public async Task<IEnumerable<Book>> GetUserFavoritesAsync(string userId)
        {
            return await _favoriteRepository.GetUserFavoriteImagesAsync(userId);
        }

        public async Task<int> GetFavoriteCountByUserAsync(string userId)
        {
            return await _favoriteRepository.GetFavoriteCountByUserAsync(userId);
        }

        public async Task<IEnumerable<Wishlist>> GetRecentFavoritesAsync(string userId, int count)
        {
            return await _favoriteRepository.GetRecentFavoritesAsync(userId, count);
        }
    }
}