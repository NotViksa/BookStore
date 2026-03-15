using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface IBookService
    {
        // Basic CRUD
        Task<IEnumerable<Book>> GetAllBooksAsync();
        Task<Book?> GetBookByIdAsync(int id);
        Task<Book> AddBookAsync(Book book, IEnumerable<string> genres);
        Task<bool> DeleteBookAsync(int id);

        // Queries
        Task<IEnumerable<Book>> GetBooksByGenreAsync(string genreName);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);
        Task<IEnumerable<Book>> GetBooksByUserAsync(string userId);
        Task<IEnumerable<Book>> GetMostPopularBooksAsync(int count);
        Task<IEnumerable<Book>> GetRecentBooksAsync(int count);
        Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // Search and counts
        Task<int> GetBookCountByUserAsync(string userId);
        Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm);
        Task<IEnumerable<Book>> GetRecentUserBooksAsync(string userId, int count);

        // User interactions
        Task AddReviewAsync(Review review);
        Task AddToWishlistAsync(Wishlist wishlist);
        Task AddRatingAsync(Rating rating);

        // Queryables for pagination
        IQueryable<Book> GetBooksQueryable();
        IQueryable<Book> GetSearchQueryable(string query);
    }
}