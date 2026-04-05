using Microsoft.EntityFrameworkCore;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using System.Linq.Expressions;

namespace BookStore.Data.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Book>> GetByTagAsync(string tagName)
        {
            return await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.UploadedBy)
                .Where(b => b.Genres.Any(g => g.Name == tagName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByUserAsync(string userId)
        {
            return await _context.Books
                .Include(b => b.Genres)
                .Where(b => b.UploadedById == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetMostPopularAsync(int count)
        {
            return await _context.Books
                .Include(b => b.Ratings)
                .OrderByDescending(b => b.Ratings.Average(r => (double?)r.Value) ?? 0)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetRecentAsync(int count)
        {
            return await _context.Books
                .OrderByDescending(b => b.AddedDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Books
                .Include(b => b.UploadedBy)
                .Include(b => b.Genres)
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.PostedBy)
                .Include(b => b.Ratings)
                .Include(b => b.Wishlists)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<int> CountAsync(Expression<Func<Book, bool>> predicate)
        {
            return await _context.Books.CountAsync(predicate);
        }

        public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetRecentAsync(20);

            return await _context.Books
                .Include(b => b.Genres)
                .Include(b => b.UploadedBy)
                .Where(b =>
                    b.Title.Contains(searchTerm) ||
                    b.Author.Contains(searchTerm) ||
                    b.Description.Contains(searchTerm) ||
                    b.ISBN.Contains(searchTerm) ||
                    b.Genres.Any(g => g.Name.Contains(searchTerm))
                )
                .ToListAsync();
        }

        public IQueryable<Book> GetQueryable()
        {
            return _context.Books.AsQueryable();
        }
        public async Task<IEnumerable<Book>> GetByAuthorAsync(string author)
        {
            return await _context.Books
                .Include(b => b.Genres)
                .Where(b => b.Author.Contains(author))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Books
                .Where(b => b.Price >= minPrice && b.Price <= maxPrice)
                .OrderBy(b => b.Price)
                .ToListAsync();
        }
    }
}