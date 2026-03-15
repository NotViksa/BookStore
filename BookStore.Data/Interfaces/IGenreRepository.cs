using BookStore.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Data.Interfaces
{
    public interface IGenreRepository
    {
        Task<IEnumerable<Genre>> GetAllAsync();
        Task<Genre?> GetByIdAsync(int id);
        Task<Genre?> GetByNameAsync(string name);
        Task<Genre> AddAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(Genre genre);
        Task<int> GetUsageCountAsync(int genreId);
        Task<IEnumerable<Genre>> GetTagsForImageAsync(int bookId);
        Task<IEnumerable<Genre>> GetPopularTagsAsync(int count);
    }
}