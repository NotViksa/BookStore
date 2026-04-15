using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;
using System.Security.Claims;
using BookStore.Web.ViewModels;

namespace BookStore.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartService cartService, IOrderService orderService, ILogger<CartController> logger)
        {
            _cartService = cartService;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetCartAsync(userId);
            var total = await _cartService.GetCartTotalAsync(userId);
            ViewBag.Total = total;
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int bookId, int quantity = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.AddToCartAsync(userId, bookId, quantity);
            TempData["SuccessMessage"] = "Book added to cart!";
            return RedirectToAction("Details", "Book", new { id = bookId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int bookId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.RemoveFromCartAsync(userId, bookId);
            TempData["SuccessMessage"] = "Item removed from cart.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int bookId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _cartService.UpdateQuantityAsync(userId, bookId, quantity);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = await _cartService.GetCartAsync(userId);
            if (!cartItems.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Index");
            }
            var total = await _cartService.GetCartTotalAsync(userId);
            ViewBag.Total = total;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.Total = await _cartService.GetCartTotalAsync(userId);
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var order = await _orderService.CreateOrderAsync(userId, model.ShippingAddress, "Fake Payment");
                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("Confirmation", new { id = order.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed");
                ModelState.AddModelError("", "Failed to place order. Please try again.");
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.Total = await _cartService.GetCartTotalAsync(userId);
                return View(model);
            }
        }

        public async Task<IActionResult> Confirmation(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();
            // Ensure the order belongs to the current user
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (order.UserId != userId && !User.IsInRole("Administrator"))
                return Forbid();
            return View(order);
        }
    }
}