using BookStore.Data.Models;
using System.Linq.Expressions;

namespace BookStore.Data.Interfaces
{
    public interface ICartRepository
    {
        Task<CartItem?> GetCartItemAsync(string userId, int bookId);
        Task<IEnumerable<CartItem>> GetUserCartAsync(string userId);
        Task AddToCartAsync(CartItem item);
        Task UpdateCartItemAsync(CartItem item);
        Task RemoveFromCartAsync(CartItem item);
        Task ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
    }
}