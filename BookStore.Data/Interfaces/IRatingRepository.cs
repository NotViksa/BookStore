using BookStore.Data.Models;
using System.Threading.Tasks;

namespace BookStore.Data.Interfaces
{
    public interface IRatingRepository
    {
        Task<Rating> GetVoteAsync(string userId, int bookId);
        Task AddVoteAsync(Rating rating);
        Task UpdateVoteAsync(Rating rating);
        Task RemoveVoteAsync(Rating rating);
        Task<int> GetImageScoreAsync(int bookId);
    }
}