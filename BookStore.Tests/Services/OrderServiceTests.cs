using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services;
using BookStore.Services.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Services
{
    public class OrderServiceTests
    {
        private readonly Mock<IOrderRepository> _mockOrderRepo;
        private readonly Mock<ICartService> _mockCartService;
        private readonly OrderService _orderService;

        public OrderServiceTests()
        {
            _mockOrderRepo = new Mock<IOrderRepository>();
            _mockCartService = new Mock<ICartService>();
            _orderService = new OrderService(_mockOrderRepo.Object, _mockCartService.Object);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidCart_ShouldCreateOrderAndClearCart()
        {
            // Arrange
            string userId = "user1";
            string address = "123 Main St";
            string payment = "Fake";

            var cartItems = new List<CartItem>
            {
                new CartItem { BookId = 1, Quantity = 2, Book = new Book { Id = 1, Price = 10m } },
                new CartItem { BookId = 2, Quantity = 1, Book = new Book { Id = 2, Price = 20m } }
            };

            _mockCartService.Setup(c => c.GetCartAsync(userId)).ReturnsAsync(cartItems);
            _mockCartService.Setup(c => c.GetCartTotalAsync(userId)).ReturnsAsync(40m);

            Order capturedOrder = null;
            _mockOrderRepo.Setup(r => r.AddOrderAsync(It.IsAny<Order>()))
                .Callback<Order>(o => capturedOrder = o)
                .Returns(Task.CompletedTask);

            // Act
            var order = await _orderService.CreateOrderAsync(userId, address, payment);

            // Assert
            Assert.NotNull(order);
            Assert.Equal(userId, order.UserId);
            Assert.Equal(40m, order.TotalAmount);
            Assert.Equal(address, order.ShippingAddress);
            Assert.Equal("Confirmed", order.OrderStatus);
            Assert.Equal(2, order.OrderItems.Count);
            _mockCartService.Verify(c => c.ClearCartAsync(userId), Times.Once);
        }

        [Fact]
        public async Task CreateOrderAsync_EmptyCart_ShouldThrowInvalidOperationException()
        {
            // Arrange
            string userId = "user1";
            _mockCartService.Setup(c => c.GetCartAsync(userId)).ReturnsAsync(new List<CartItem>());

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _orderService.CreateOrderAsync(userId, "address", "payment"));
        }

        [Fact]
        public async Task GetUserOrdersAsync_ShouldReturnOrders()
        {
            // Arrange
            string userId = "user1";
            var orders = new List<Order> { new Order(), new Order() };
            _mockOrderRepo.Setup(r => r.GetUserOrdersAsync(userId)).ReturnsAsync(orders);

            // Act
            var result = await _orderService.GetUserOrdersAsync(userId);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetOrderByIdAsync_ShouldReturnOrder()
        {
            // Arrange
            int orderId = 1;
            var order = new Order { Id = orderId };
            _mockOrderRepo.Setup(r => r.GetOrderByIdAsync(orderId)).ReturnsAsync(order);

            // Act
            var result = await _orderService.GetOrderByIdAsync(orderId);

            // Assert
            Assert.Equal(orderId, result.Id);
        }
    }
}