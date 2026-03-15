using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _wishlistRepository;
        private readonly ILogger<WishlistService> _logger;

        public WishlistService(
            IWishlistRepository wishlistRepository,
            ILogger<WishlistService> logger)
        {
            _wishlistRepository = wishlistRepository;
            _logger = logger;
        }

        public async Task<bool> ToggleWishlistAsync(string userId, int bookId)
        {
            try
            {
                var existing = await _wishlistRepository.GetFavoriteAsync(userId, bookId);

                if (existing != null)
                {
                    await _wishlistRepository.RemoveFavoriteAsync(existing);
                    return false;
                }

                var wishlist = new Wishlist
                {
                    UserId = userId,
                    BookId = bookId,
                    AddedDate = System.DateTime.UtcNow
                };

                await _wishlistRepository.AddFavoriteAsync(wishlist);
                return true; // Added to wishlist
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling wishlist for user {UserId} and book {BookId}", userId, bookId);
                throw;
            }
        }

        public async Task<bool> IsInWishlistAsync(string userId, int bookId)
        {
            return await _wishlistRepository.FavoriteExistsAsync(userId, bookId);
        }

        public async Task<IEnumerable<Book>> GetUserWishlistAsync(string userId)
        {
            return await _wishlistRepository.GetUserFavoriteImagesAsync(userId);
        }

        public async Task<int> GetWishlistCountByUserAsync(string userId)
        {
            return await _wishlistRepository.GetFavoriteCountByUserAsync(userId);
        }

        public async Task<IEnumerable<Wishlist>> GetRecentWishlistItemsAsync(string userId, int count)
        {
            return await _wishlistRepository.GetRecentFavoritesAsync(userId, count);
        }

        public async Task<bool> ClearWishlistAsync(string userId)
        {
            try
            {
                var wishlistItems = await _wishlistRepository.GetRecentFavoritesAsync(userId, int.MaxValue);
                foreach (var item in wishlistItems)
                {
                    await _wishlistRepository.RemoveFavoriteAsync(item);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing wishlist for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(string userId, int bookId)
        {
            try
            {
                var existing = await _wishlistRepository.GetFavoriteAsync(userId, bookId);
                if (existing != null)
                {
                    await _wishlistRepository.RemoveFavoriteAsync(existing);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing book {BookId} from wishlist for user {UserId}", bookId, userId);
                return false;
            }
        }
    }
}