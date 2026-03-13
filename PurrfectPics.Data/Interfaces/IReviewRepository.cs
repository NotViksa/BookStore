using BookStore.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Data.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetByIdAsync(int commentId);
        Task<Review> AddAsync(Review comment);
        Task<bool> DeleteAsync(Review comment);
        Task<IEnumerable<Review>> GetCommentsForImageAsync(int catImageId);
        Task<int> GetCountByUserAsync(string userId);
        Task<bool> UpdateAsync(Review comment);
    }
}