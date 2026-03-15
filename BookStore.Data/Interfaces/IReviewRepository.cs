using BookStore.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Data.Interfaces
{
    public interface IReviewRepository
    {
        Task<Review> GetByIdAsync(int reviewId);
        Task<Review> AddAsync(Review review);
        Task<bool> DeleteAsync(Review review);
        Task<IEnumerable<Review>> GetCommentsForImageAsync(int bookId);
        Task<int> GetCountByUserAsync(string userId);
        Task<bool> UpdateAsync(Review review);
    }
}