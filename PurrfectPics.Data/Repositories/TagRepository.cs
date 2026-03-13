using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data;
using PurrfectPics.Data.Interfaces;
using PurrfectPics.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PurrfectPics.Data.Repositories
{
    public class TagRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> AddAsync(Genre tag)
        {
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();
            return tag;
        }

        public async Task DeleteAsync(Genre tag)
        {
            _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<Genre?> GetByNameAsync(string name)
        {
            return await _context.Tags
                .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Genre>> GetPopularTagsAsync(int count)
        {
            return await _context.Tags
                .Include(t => t.CatImages)
                .OrderByDescending(t => t.CatImages.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Genre>> GetTagsForImageAsync(int imageId)
        {
            return await _context.Tags
                .Where(t => t.CatImages.Any(ci => ci.Id == imageId))
                .ToListAsync();
        }

        public async Task<int> GetUsageCountAsync(int tagId)
        {
            return await _context.Tags
                .Where(t => t.Id == tagId)
                .Select(t => t.CatImages.Count)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Genre tag)
        {
            _context.Tags.Update(tag);
            await _context.SaveChangesAsync();
        }
    }
}