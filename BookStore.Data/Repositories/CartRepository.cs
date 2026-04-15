using Microsoft.EntityFrameworkCore;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;

namespace BookStore.Data.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CartItem?> GetCartItemAsync(string userId, int bookId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);
        }

        public async Task<IEnumerable<CartItem>> GetUserCartAsync(string userId)
        {
            return await _context.CartItems
                .Include(c => c.Book)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task AddToCartAsync(CartItem item)
        {
            await _context.CartItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(CartItem item)
        {
            _context.CartItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(CartItem item)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task ClearCartAsync(string userId)
        {
            var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            return await _context.CartItems.CountAsync(c => c.UserId == userId);
        }
    }
}