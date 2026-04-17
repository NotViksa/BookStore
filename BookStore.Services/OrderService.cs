using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICartService _cartService;

        public OrderService(IOrderRepository orderRepository, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _cartService = cartService;
        }

        public async Task<Order> CreateOrderAsync(string userId, string shippingAddress, string paymentMethod)
        {
            var cartItems = await _cartService.GetCartAsync(userId);
            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            var total = await _cartService.GetCartTotalAsync(userId);
            var order = new Order
            {
                UserId = userId,
                OrderDate = DateTime.UtcNow,
                TotalAmount = total,
                ShippingAddress = shippingAddress,
                PaymentMethod = paymentMethod,
                OrderStatus = "Confirmed"
            };

            foreach (var item in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Book.Price
                });
            }

            await _orderRepository.AddOrderAsync(order);
            await _cartService.ClearCartAsync(userId);
            return order;
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId)
        {
            return await _orderRepository.GetUserOrdersAsync(userId);
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderByIdAsync(orderId);
        }
        public IQueryable<Book> GetPurchasedBooksQueryable(string userId)
        {
            return _orderRepository.GetPurchasedBooksQueryable(userId);
        }
        public async Task<IEnumerable<Book>> GetPurchasedBooksAsync(string userId)
        {
            return await _orderRepository.GetPurchasedBooksQueryable(userId).ToListAsync();
        }
        public int GetAllOrdersCount()
        {
            return _orderRepository.GetAllOrdersCount();
        }
    }
}