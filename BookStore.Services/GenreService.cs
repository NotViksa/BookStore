using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _genreRepository;
        private readonly ILogger<GenreService> _logger;

        public GenreService(
            IGenreRepository genreRepository,
            ILogger<GenreService> logger)
        {
            _genreRepository = genreRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _genreRepository.GetAllAsync();
        }

        public async Task<Genre?> GetGenreByIdAsync(int id)
        {
            return await _genreRepository.GetByIdAsync(id);
        }

        public async Task<Genre?> GetGenreByNameAsync(string name)
        {
            return await _genreRepository.GetByNameAsync(name);
        }

        public async Task<Genre> CreateGenreAsync(string name, string description = null)
        {
            var existingGenre = await _genreRepository.GetByNameAsync(name);
            if (existingGenre != null)
            {
                return existingGenre;
            }

            var newGenre = new Genre
            {
                Name = name,
                Description = description
            };

            return await _genreRepository.AddAsync(newGenre);
        }

        public async Task<Genre> UpdateGenreAsync(int id, string name, string description)
        {
            var genre = await _genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                throw new KeyNotFoundException($"Genre with ID {id} not found");
            }

            genre.Name = name;
            genre.Description = description;

            await _genreRepository.UpdateAsync(genre);
            return genre;
        }

        public async Task<bool> DeleteGenreAsync(int id)
        {
            try
            {
                var genre = await _genreRepository.GetByIdAsync(id);
                if (genre == null) return false;

                await _genreRepository.DeleteAsync(genre);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting genre with ID {GenreId}", id);
                return false;
            }
        }

        public async Task<IEnumerable<Genre>> GetPopularGenresAsync(int count)
        {
            return await _genreRepository.GetPopularTagsAsync(count);
        }

        public async Task<IEnumerable<Genre>> GetGenresForBookAsync(int bookId)
        {
            return await _genreRepository.GetTagsForImageAsync(bookId);
        }

        public async Task<int> GetGenreUsageCountAsync(int genreId)
        {
            return await _genreRepository.GetUsageCountAsync(genreId);
        }

        public async Task<Genre> EnsureGenreExistsAsync(string name)
        {
            return await CreateGenreAsync(name);
        }

        public async Task AddGenreToBookAsync(int bookId, int genreId)
        {
            // This would need a repository method to handle many-to-many relationship
            // Implementation depends on how you want to manage this
            throw new System.NotImplementedException();
        }

        public async Task RemoveGenreFromBookAsync(int bookId, int genreId)
        {
            // This would need a repository method to handle many-to-many relationship
            throw new System.NotImplementedException();
        }
    }
}