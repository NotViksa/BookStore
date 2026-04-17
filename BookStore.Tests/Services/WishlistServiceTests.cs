using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Services
{
    public class WishlistServiceTests
    {
        private readonly Mock<IWishlistRepository> _mockWishlistRepo;
        private readonly Mock<ILogger<WishlistService>> _mockLogger;
        private readonly WishlistService _wishlistService;

        public WishlistServiceTests()
        {
            _mockWishlistRepo = new Mock<IWishlistRepository>();
            _mockLogger = new Mock<ILogger<WishlistService>>();
            _wishlistService = new WishlistService(_mockWishlistRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ToggleWishlistAsync_NotInWishlist_ShouldAddAndReturnTrue()
        {
            // Arrange
            string userId = "user1";
            int bookId = 1;
            _mockWishlistRepo.Setup(r => r.GetFavoriteAsync(userId, bookId))
                .ReturnsAsync((Wishlist?)null);

            // Act
            var result = await _wishlistService.ToggleWishlistAsync(userId, bookId);

            // Assert
            Assert.True(result);
            _mockWishlistRepo.Verify(r => r.AddFavoriteAsync(It.Is<Wishlist>(
                w => w.UserId == userId && w.BookId == bookId)), Times.Once);
            _mockWishlistRepo.Verify(r => r.RemoveFavoriteAsync(It.IsAny<Wishlist>()), Times.Never);
        }

        [Fact]
        public async Task ToggleWishlistAsync_AlreadyInWishlist_ShouldRemoveAndReturnFalse()
        {
            // Arrange
            string userId = "user1";
            int bookId = 1;
            var existing = new Wishlist { UserId = userId, BookId = bookId };
            _mockWishlistRepo.Setup(r => r.GetFavoriteAsync(userId, bookId))
                .ReturnsAsync(existing);

            // Act
            var result = await _wishlistService.ToggleWishlistAsync(userId, bookId);

            // Assert
            Assert.False(result);
            _mockWishlistRepo.Verify(r => r.RemoveFavoriteAsync(existing), Times.Once);
            _mockWishlistRepo.Verify(r => r.AddFavoriteAsync(It.IsAny<Wishlist>()), Times.Never);
        }

        [Fact]
        public async Task IsInWishlistAsync_WhenExists_ReturnsTrue()
        {
            // Arrange
            _mockWishlistRepo.Setup(r => r.FavoriteExistsAsync("user1", 1)).ReturnsAsync(true);

            // Act
            var result = await _wishlistService.IsInWishlistAsync("user1", 1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetUserWishlistAsync_ShouldReturnBooks()
        {
            // Arrange
            var books = new List<Book> { new Book { Id = 1 }, new Book { Id = 2 } };
            _mockWishlistRepo.Setup(r => r.GetUserFavoriteImagesAsync("user1")).ReturnsAsync(books);

            // Act
            var result = await _wishlistService.GetUserWishlistAsync("user1");

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task ClearWishlistAsync_WithItems_ShouldRemoveAll()
        {
            // Arrange
            string userId = "user1";
            var items = new List<Wishlist> { new Wishlist(), new Wishlist() };
            _mockWishlistRepo.Setup(r => r.GetRecentFavoritesAsync(userId, int.MaxValue))
                .ReturnsAsync(items);

            // Act
            var result = await _wishlistService.ClearWishlistAsync(userId);

            // Assert
            Assert.True(result);
            _mockWishlistRepo.Verify(r => r.RemoveFavoriteAsync(It.IsAny<Wishlist>()), Times.Exactly(2));
        }

        [Fact]
        public async Task ToggleWishlistAsync_WhenException_ShouldThrow()
        {
            // Arrange
            _mockWishlistRepo.Setup(r => r.GetFavoriteAsync("user1", 1))
                .ThrowsAsync(new Exception("DB error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _wishlistService.ToggleWishlistAsync("user1", 1));
        }
    }
}