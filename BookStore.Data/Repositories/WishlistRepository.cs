using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Data.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Wishlist> GetFavoriteAsync(string userId, int bookId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.BookId == bookId);
        }

        public async Task AddFavoriteAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(Wishlist wishlist)
        {
            _context.Wishlists.Remove(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> FavoriteExistsAsync(string userId, int bookId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.UserId == userId && w.BookId == bookId);
        }

        public async Task<IEnumerable<Book>> GetUserFavoriteImagesAsync(string userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Book)
                    .ThenInclude(b => b.Genres)
                .Select(w => w.Book)
                .ToListAsync();
        }

        public async Task<int> GetFavoriteCountByUserAsync(string userId)
        {
            return await _context.Wishlists
                .CountAsync(w => w.UserId == userId);
        }

        public async Task<IEnumerable<Wishlist>> GetRecentFavoritesAsync(string userId, int count)
        {
            return await _context.Wishlists
                .Include(w => w.Book)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedDate)
                .Take(count)
                .ToListAsync();
        }
        public IQueryable<Book> GetUserWishlistQueryable(string userId)
        {
            return _context.Wishlists
                .Where(w => w.UserId == userId)
                .Include(w => w.Book)
                    .ThenInclude(b => b.Genres)
                .Include(w => w.Book)
                    .ThenInclude(b => b.Reviews)
                .Select(w => w.Book)
                .AsQueryable();
        }
    }
}