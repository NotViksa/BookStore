using BookStore.Data.Models;
using System.Linq.Expressions;

namespace BookStore.Data.Interfaces
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<IEnumerable<Book>> GetByTagAsync(string tagName);
        Task<IEnumerable<Book>> GetByUserAsync(string userId);
        Task<IEnumerable<Book>> GetMostPopularAsync(int count);
        Task<IEnumerable<Book>> GetRecentAsync(int count);
        Task<Book?> GetByIdWithDetailsAsync(int id);
        Task<int> CountAsync(Expression<Func<Book, bool>> predicate);
        Task<IEnumerable<Book>> SearchAsync(string searchTerm);
        IQueryable<Book> GetQueryable();
        Task<IEnumerable<Book>> GetBooksByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Book>> GetByAuthorAsync(string author);
    }
}