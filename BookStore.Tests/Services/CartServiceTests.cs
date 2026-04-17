using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Services
{
    public class CartServiceTests
    {
        private readonly Mock<ICartRepository> _mockCartRepo;
        private readonly Mock<IBookRepository> _mockBookRepo;
        private readonly CartService _cartService;

        public CartServiceTests()
        {
            _mockCartRepo = new Mock<ICartRepository>();
            _mockBookRepo = new Mock<IBookRepository>();
            _cartService = new CartService(_mockCartRepo.Object, _mockBookRepo.Object);
        }

        [Fact]
        public async Task AddToCartAsync_NewItem_ShouldAddNewCartItem()
        {
            // Arrange
            string userId = "user1";
            int bookId = 1;
            _mockCartRepo.Setup(r => r.GetCartItemAsync(userId, bookId))
                .ReturnsAsync((CartItem?)null);

            // Act
            await _cartService.AddToCartAsync(userId, bookId, 2);

            // Assert
            _mockCartRepo.Verify(r => r.AddToCartAsync(It.Is<CartItem>(
                c => c.UserId == userId && c.BookId == bookId && c.Quantity == 2)), Times.Once);
        }

        [Fact]
        public async Task AddToCartAsync_ExistingItem_ShouldUpdateQuantity()
        {
            // Arrange
            string userId = "user1";
            int bookId = 1;
            var existingItem = new CartItem { UserId = userId, BookId = bookId, Quantity = 1 };
            _mockCartRepo.Setup(r => r.GetCartItemAsync(userId, bookId))
                .ReturnsAsync(existingItem);

            // Act
            await _cartService.AddToCartAsync(userId, bookId, 3);

            // Assert
            Assert.Equal(4, existingItem.Quantity);
            _mockCartRepo.Verify(r => r.UpdateCartItemAsync(existingItem), Times.Once);
            _mockCartRepo.Verify(r => r.AddToCartAsync(It.IsAny<CartItem>()), Times.Never);
        }

        [Fact]
        public async Task UpdateQuantityAsync_ZeroQuantity_ShouldRemoveItem()
        {
            // Arrange
            string userId = "user1";
            int bookId = 1;
            var item = new CartItem { UserId = userId, BookId = bookId, Quantity = 2 };
            _mockCartRepo.Setup(r => r.GetCartItemAsync(userId, bookId))
                .ReturnsAsync(item);

            // Act
            await _cartService.UpdateQuantityAsync(userId, bookId, 0);

            // Assert
            _mockCartRepo.Verify(r => r.RemoveFromCartAsync(item), Times.Once);
        }

        [Fact]
        public async Task GetCartTotalAsync_ShouldCalculateCorrectTotal()
        {
            // Arrange
            string userId = "user1";
            var items = new[]
            {
                new CartItem { Book = new Book { Price = 10m }, Quantity = 2 },
                new CartItem { Book = new Book { Price = 5m }, Quantity = 1 }
            };
            _mockCartRepo.Setup(r => r.GetUserCartAsync(userId))
                .ReturnsAsync(items);

            // Act
            var total = await _cartService.GetCartTotalAsync(userId);

            // Assert
            Assert.Equal(25m, total);
        }

        [Fact]
        public async Task RemoveFromCartAsync_ExistingItem_ShouldCallRepository()
        {
            var item = new CartItem { UserId = "u1", BookId = 1 };
            _mockCartRepo.Setup(r => r.GetCartItemAsync("u1", 1)).ReturnsAsync(item);
            await _cartService.RemoveFromCartAsync("u1", 1);
            _mockCartRepo.Verify(r => r.RemoveFromCartAsync(item), Times.Once);
        }
    }
}