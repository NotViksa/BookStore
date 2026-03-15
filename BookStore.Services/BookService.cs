using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace BookStore.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IGenreService _genreService;
        private readonly ILogger<BookService> _logger;

        public BookService(
            IBookRepository bookRepository,
            IGenreService genreService,
            ILogger<BookService> logger)
        {
            _bookRepository = bookRepository;
            _genreService = genreService;
            _logger = logger;
        }

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<Book?> GetBookByIdAsync(int id)
        {
            return await _bookRepository.GetByIdWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Book>> GetBooksByGenreAsync(string genreName)
        {
            return await _bookRepository.GetByTagAsync(genreName);
        }

        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
        {
            return await _bookRepository.GetByAuthorAsync(author);
        }

        public async Task<IEnumerable<Book>> GetBooksByUserAsync(string userId)
        {
            return await _bookRepository.GetByUserAsync(userId);
        }

        public async Task<IEnumerable<Book>> GetMostPopularBooksAsync(int count)
        {
            return await _bookRepository.GetMostPopularAsync(count);
        }

        public async Task<IEnumerable<Book>> GetRecentBooksAsync(int count)
        {
            return await GetBooksQueryable()
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _bookRepository.GetBooksByPriceRangeAsync(minPrice, maxPrice);
        }

        public async Task<Book> AddBookAsync(Book book, IEnumerable<string> genres)
        {
            var genreList = new List<Genre>();

            foreach (var genreName in genres)
            {
                var genre = await _genreService.EnsureGenreExistsAsync(genreName.Trim());
                genreList.Add(genre);
            }

            book.Genres = genreList;
            await _bookRepository.AddAsync(book);
            return book;
        }

        public async Task AddReviewAsync(Review review)
        {
            await _bookRepository.UpdateAsync(review.Book);
        }

        public async Task AddToWishlistAsync(Wishlist wishlist)
        {
            await _bookRepository.UpdateAsync(wishlist.Book);
        }

        public async Task AddRatingAsync(Rating rating)
        {
            await _bookRepository.UpdateAsync(rating.Book);
        }

        public async Task<bool> DeleteBookAsync(int id)
        {
            try
            {
                var book = await _bookRepository.GetByIdWithDetailsAsync(id);
                if (book == null) return false;

                // Clear collections to avoid foreign key issues
                book.Reviews?.Clear();
                book.Ratings?.Clear();
                book.Wishlists?.Clear();

                await _bookRepository.DeleteAsync(id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting book with ID {BookId}", id);
                return false;
            }
        }

        public async Task<int> GetBookCountByUserAsync(string userId)
        {
            return await _bookRepository.CountAsync(b => b.UploadedById == userId);
        }

        public async Task<IEnumerable<Book>> SearchBooksAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetRecentBooksAsync(20);

            return await _bookRepository.SearchAsync(searchTerm);
        }

        public async Task<IEnumerable<Book>> GetRecentUserBooksAsync(string userId, int count)
        {
            return await _bookRepository.FindAsync(
                b => b.UploadedById == userId,
                orderBy: q => q.OrderByDescending(b => b.AddedDate),
                take: count
            );
        }

        public IQueryable<Book> GetBooksQueryable()
        {
            return _bookRepository.GetQueryable()
                .Include(b => b.Genres)
                .Include(b => b.UploadedBy)
                .OrderByDescending(b => b.AddedDate);
        }

        public IQueryable<Book> GetSearchQueryable(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetBooksQueryable();

            query = query.ToLower();
            return _bookRepository.GetQueryable()
                .Include(b => b.Genres)
                .Include(b => b.UploadedBy)
                .Where(b => b.Title.ToLower().Contains(query) ||
                           b.Author.ToLower().Contains(query) ||
                           b.Description.ToLower().Contains(query) ||
                           b.ISBN.ToLower().Contains(query) ||
                           b.Genres.Any(g => g.Name.ToLower().Contains(query)))
                .OrderByDescending(b => b.AddedDate);
        }
    }
}