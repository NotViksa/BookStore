using BookStore.Data.Models;

namespace BookStore.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string userId, string shippingAddress, string paymentMethod);
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int orderId);
        IQueryable<Book> GetPurchasedBooksQueryable(string userId);
        Task<IEnumerable<Book>> GetPurchasedBooksAsync(string id);
    }
}