using BookStore.Data.Models;
using BookStore.Data.Models.Identity;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IBookService _bookService;
        private readonly IOrderService _orderService;
        private readonly IReviewService _reviewService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IBookService bookService,
            IOrderService orderService,
            IReviewService reviewService,
            ILogger<AdminController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _bookService = bookService;
            _orderService = orderService;
            _reviewService = reviewService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var stats = new
            {
                TotalUsers = _userManager.Users.Count(),
                TotalBooks = _bookService.GetBooksQueryable().Count(),
                TotalOrders = _orderService.GetAllOrdersCount(),
                TotalReviews = _reviewService.GetTotalReviewsCount()
            };
            ViewBag.Stats = stats;
            return View();
        }

        // ========== USER MANAGEMENT ==========
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _userManager.Users
                .OrderByDescending(u => u.RegistrationDate)
                .ToListAsync();

            var userRoles = new Dictionary<string, IList<string>>();
            foreach (var user in users)
            {
                userRoles[user.Id] = await _userManager.GetRolesAsync(user);
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdminRole(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var isAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
            if (isAdmin)
                await _userManager.RemoveFromRoleAsync(user, "Administrator");
            else
                await _userManager.AddToRoleAsync(user, "Administrator");

            TempData["SuccessMessage"] = $"Admin role {(isAdmin ? "removed from" : "added to")} {user.DisplayName}";
            return RedirectToAction(nameof(ManageUsers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Prevent deleting the last admin
            if (await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                var adminCount = (await _userManager.GetUsersInRoleAsync("Administrator")).Count;
                if (adminCount <= 1)
                {
                    TempData["ErrorMessage"] = "Cannot delete the last administrator.";
                    return RedirectToAction(nameof(ManageUsers));
                }
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                TempData["SuccessMessage"] = $"User {user.DisplayName} deleted.";
            else
                TempData["ErrorMessage"] = "Failed to delete user.";

            return RedirectToAction(nameof(ManageUsers));
        }

        // ========== BOOK MANAGEMENT ==========
        public IActionResult ManageBooks()
        {
            var books = _bookService.GetBooksQueryable()
                .Include(b => b.UploadedBy)
                .OrderByDescending(b => b.AddedDate)
                .ToList();
            return View(books);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var success = await _bookService.DeleteBookAsync(id);
            if (success)
                TempData["SuccessMessage"] = "Book deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to delete book.";

            return RedirectToAction(nameof(ManageBooks));
        }

        // ========== ERROR SIMULATION (for demo) ==========
        [AllowAnonymous]
        public IActionResult SimulateError()
        {
            // This triggers the status code middleware, which redirects to /Home/Error?statusCode=500
            return StatusCode(500);
        }
    }
}