using Microsoft.EntityFrameworkCore;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using System.Linq.Expressions;

namespace BookStore.Data.Repositories
{
    public class CatImageRepository : Repository<Book>, IBookRepository
    {
        public CatImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Book>> GetByTagAsync(string tagName)
        {
            return await _context.CatImages
                .Include(ci => ci.Tags)
                .Include(ci => ci.UploadedBy)
                .Where(ci => ci.Tags.Any(t => t.Name == tagName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetByUserAsync(string userId)
        {
            return await _context.CatImages
                .Include(ci => ci.Tags)
                .Where(ci => ci.UploadedById == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetMostPopularAsync(int count)
        {
            return await _context.CatImages
                .Include(ci => ci.Votes)
                .OrderByDescending(ci => ci.Votes.Count(v => v.IsUpvote) - ci.Votes.Count(v => !v.IsUpvote))
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetRecentAsync(int count)
        {
            return await _context.CatImages
                .OrderByDescending(ci => ci.UploadDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task<Book?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.CatImages
                .Include(ci => ci.UploadedBy)
                .Include(ci => ci.Tags)
                .Include(ci => ci.Comments)
                .Include(ci => ci.Votes)
                .Include(ci => ci.Favorites)
                .FirstOrDefaultAsync(ci => ci.Id == id);
        }

        public async Task<int> CountAsync(Expression<Func<Book, bool>> predicate)
        {
            return await _context.CatImages.CountAsync(predicate);
        }
        public async Task<IEnumerable<Book>> SearchAsync(string searchTerm)
        {
            return await _context.CatImages
                .Include(ci => ci.Tags)
                .Include(ci => ci.UploadedBy)
                .Where(ci =>
                    ci.Title.Contains(searchTerm) ||
                    ci.Description.Contains(searchTerm) ||
                    ci.Tags.Any(t => t.Name.Contains(searchTerm))
                )
                .ToListAsync();
        }
        public IQueryable<Book> GetQueryable()
        {
            return _context.CatImages.AsQueryable();
        }
    }
}