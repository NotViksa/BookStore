using Microsoft.EntityFrameworkCore;
using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface IBookService
    {
        Task<IEnumerable<Book>> GetAllImagesAsync();
        Task<Book?> GetImageByIdAsync(int id);
        Task<IEnumerable<Book>> GetImagesByTagAsync(string tagName);
        Task<IEnumerable<Book>> GetImagesByUserAsync(string userId);
        Task<IEnumerable<Book>> GetMostPopularImagesAsync(int count);
        Task<IEnumerable<Book>> GetRecentImagesAsync(int count);
        Task<Book> AddImageAsync(Book image, IEnumerable<string> tags);
        Task AddCommentAsync(Review comment);
        Task AddFavoriteAsync(Wishlist favorite);
        Task AddVoteAsync(Rating vote);
        Task<bool> DeleteImageAsync(int id);
        Task<int> GetImageCountByUserAsync(string userId);
        Task<IEnumerable<Book>> SearchImagesAsync(string searchTerm);
        Task<IEnumerable<Book>> GetRecentUserImagesAsync(string userId, int count);
        IQueryable<Book> GetImagesQueryable();
        IQueryable<Book> GetSearchQueryable(string query);

    }
}