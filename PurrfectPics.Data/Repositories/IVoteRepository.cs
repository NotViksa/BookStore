using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Data.Repositories
{
    public class VoteRepository : IRatingRepository
    {
        private readonly ApplicationDbContext _context;

        public VoteRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Rating> GetVoteAsync(string userId, int imageId)
        {
            return await _context.Votes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.CatImageId == imageId);
        }

        public async Task AddVoteAsync(Rating vote)
        {
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVoteAsync(Rating vote)
        {
            _context.Votes.Update(vote);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveVoteAsync(Rating vote)
        {
            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetImageScoreAsync(int imageId)
        {
            var votes = await _context.Votes
                .Where(v => v.CatImageId == imageId)
                .ToListAsync();

            return votes.Sum(v => v.IsUpvote ? 1 : -1);
        }
    }
}