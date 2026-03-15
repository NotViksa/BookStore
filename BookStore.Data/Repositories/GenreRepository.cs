using Microsoft.EntityFrameworkCore;
using BookStore.Data;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Data.Repositories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly ApplicationDbContext _context;

        public GenreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> AddAsync(Genre genre)
        {
            await _context.Genres.AddAsync(genre);
            await _context.SaveChangesAsync();
            return genre;
        }

        public async Task DeleteAsync(Genre genre)
        {
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Genre>> GetAllAsync()
        {
            return await _context.Genres
                .OrderBy(g => g.Name)
                .ToListAsync();
        }

        public async Task<Genre?> GetByIdAsync(int id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task<Genre?> GetByNameAsync(string name)
        {
            return await _context.Genres
                .FirstOrDefaultAsync(g => g.Name.ToLower() == name.ToLower());
        }

        public async Task<IEnumerable<Genre>> GetPopularTagsAsync(int count)
        {
            return await _context.Genres
                .Include(g => g.Books)
                .OrderByDescending(g => g.Books.Count)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Genre>> GetTagsForImageAsync(int bookId)
        {
            return await _context.Genres
                .Where(g => g.Books.Any(b => b.Id == bookId))
                .ToListAsync();
        }

        public async Task<int> GetUsageCountAsync(int genreId)
        {
            return await _context.Genres
                .Where(g => g.Id == genreId)
                .Select(g => g.Books.Count)
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }
    }
}