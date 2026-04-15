using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;

namespace BookStore.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IBookRepository _bookRepository;

        public CartService(ICartRepository cartRepository, IBookRepository bookRepository)
        {
            _cartRepository = cartRepository;
            _bookRepository = bookRepository;
        }

        public async Task AddToCartAsync(string userId, int bookId, int quantity = 1)
        {
            var existing = await _cartRepository.GetCartItemAsync(userId, bookId);
            if (existing != null)
            {
                existing.Quantity += quantity;
                await _cartRepository.UpdateCartItemAsync(existing);
            }
            else
            {
                var item = new CartItem
                {
                    UserId = userId,
                    BookId = bookId,
                    Quantity = quantity,
                    AddedDate = DateTime.UtcNow
                };
                await _cartRepository.AddToCartAsync(item);
            }
        }

        public async Task RemoveFromCartAsync(string userId, int bookId)
        {
            var item = await _cartRepository.GetCartItemAsync(userId, bookId);
            if (item != null)
                await _cartRepository.RemoveFromCartAsync(item);
        }

        public async Task UpdateQuantityAsync(string userId, int bookId, int quantity)
        {
            if (quantity <= 0)
            {
                await RemoveFromCartAsync(userId, bookId);
                return;
            }
            var item = await _cartRepository.GetCartItemAsync(userId, bookId);
            if (item != null)
            {
                item.Quantity = quantity;
                await _cartRepository.UpdateCartItemAsync(item);
            }
        }

        public async Task<IEnumerable<CartItem>> GetCartAsync(string userId)
        {
            return await _cartRepository.GetUserCartAsync(userId);
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _cartRepository.GetCartItemCountAsync(userId);
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var items = await GetCartAsync(userId);
            return items.Sum(i => i.Book.Price * i.Quantity);
        }

        public async Task ClearCartAsync(string userId)
        {
            await _cartRepository.ClearCartAsync(userId);
        }
    }
}