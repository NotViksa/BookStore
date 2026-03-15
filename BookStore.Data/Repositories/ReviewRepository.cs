using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly ApplicationDbContext _context;

        public ReviewRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Review> AddAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<bool> DeleteAsync(Review review)
        {
            _context.Reviews.Remove(review);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Review> GetByIdAsync(int reviewId)
        {
            return await _context.Reviews
                .Include(r => r.PostedBy)
                .FirstOrDefaultAsync(r => r.Id == reviewId);
        }

        public async Task<IEnumerable<Review>> GetCommentsForImageAsync(int bookId)
        {
            return await _context.Reviews
                .Include(r => r.PostedBy)
                .Where(r => r.BookId == bookId)
                .OrderByDescending(r => r.PostedDate)
                .ToListAsync();
        }

        public async Task<int> GetCountByUserAsync(string userId)
        {
            return await _context.Reviews
                .CountAsync(r => r.PostedById == userId);
        }

        public async Task<bool> UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}