using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Data.Repositories
{
    public class RatingRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public RatingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Rating> GetVoteAsync(string userId, int bookId)
        {
            return await _context.Ratings
                .FirstOrDefaultAsync(r => r.UserId == userId && r.BookId == bookId);
        }

        public async Task AddVoteAsync(Rating rating)
        {
            await _context.Ratings.AddAsync(rating);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVoteAsync(Rating rating)
        {
            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveVoteAsync(Rating rating)
        {
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetImageScoreAsync(int bookId)
        {
            var ratings = await _context.Ratings
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            if (!ratings.Any())
                return 0;

            return (int)ratings.Average(r => r.Value);
        }
    }
}