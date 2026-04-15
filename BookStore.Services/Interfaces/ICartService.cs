using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int bookId, int quantity = 1);
        Task RemoveFromCartAsync(string userId, int bookId);
        Task UpdateQuantityAsync(string userId, int bookId, int quantity);
        Task<IEnumerable<CartItem>> GetCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
        Task ClearCartAsync(string userId);
    }
}