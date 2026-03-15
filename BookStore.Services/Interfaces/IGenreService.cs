using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface IGenreService
    {
        // Basic CRUD
        Task<IEnumerable<Genre>> GetAllGenresAsync();
        Task<Genre?> GetGenreByIdAsync(int id);
        Task<Genre?> GetGenreByNameAsync(string name);
        Task<Genre> CreateGenreAsync(string name, string description = null);
        Task<Genre> UpdateGenreAsync(int id, string name, string description);
        Task<bool> DeleteGenreAsync(int id);

        // Genre operations
        Task<IEnumerable<Genre>> GetPopularGenresAsync(int count);
        Task<IEnumerable<Genre>> GetGenresForBookAsync(int bookId);
        Task<int> GetGenreUsageCountAsync(int genreId);
        Task<Genre> EnsureGenreExistsAsync(string name);

        // Book-genre management
        Task AddGenreToBookAsync(int bookId, int genreId);
        Task RemoveGenreFromBookAsync(int bookId, int genreId);
    }
}