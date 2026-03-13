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
        Task<Genre> AddAsync(Genre tag);
        Task UpdateAsync(Genre tag);
        Task DeleteAsync(Genre tag);
        Task<int> GetUsageCountAsync(int tagId);
        Task<IEnumerable<Genre>> GetTagsForImageAsync(int imageId);
        Task<IEnumerable<Genre>> GetPopularTagsAsync(int count);
    }
}