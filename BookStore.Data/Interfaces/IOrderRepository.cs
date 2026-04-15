using BookStore.Data.Models;

namespace BookStore.Data.Interfaces
{
    public interface IOrderRepository
    {
        Task AddOrderAsync(Order order);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        IQueryable<Book> GetPurchasedBooksQueryable(string userId);
    }
}