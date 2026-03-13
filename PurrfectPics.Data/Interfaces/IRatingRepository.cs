using BookStore.Data.Models;
using System.Threading.Tasks;

namespace BookStore.Data.Interfaces
{
    public interface IRatingRepository
    {
        Task<Rating> GetVoteAsync(string userId, int imageId);
        Task AddVoteAsync(Rating vote);
        Task UpdateVoteAsync(Rating vote);
        Task RemoveVoteAsync(Rating vote);
        Task<int> GetImageScoreAsync(int imageId);
    }
}