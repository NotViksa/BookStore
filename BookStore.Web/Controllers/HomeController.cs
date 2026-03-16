using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data.Models.Identity;
using BookStore.Services.Interfaces;
using BookStore.Web.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace BookStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IGenreService _genreService;
        private readonly IWishlistService _wishlistService;
        private readonly IReviewService _reviewService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            IBookService bookService,
            IGenreService genreService,
            IWishlistService wishlistService,
            IReviewService reviewService,
            UserManager<ApplicationUser> userManager,
            ILogger<HomeController> logger)
        {
            _bookService = bookService;
            _genreService = genreService;
            _wishlistService = wishlistService;
            _reviewService = reviewService;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var recentBooks = await _bookService.GetRecentBooksAsync(12);
                var popularGenres = await _genreService.GetPopularGenresAsync(10);
                var popularBooks = await _bookService.GetMostPopularBooksAsync(6);

                ViewBag.RecentBooks = recentBooks;
                ViewBag.PopularGenres = popularGenres;
                ViewBag.PopularBooks = popularBooks;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page");
                return View();
            }
        }

        public IActionResult About()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Dashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            ViewBag.UserDisplayName = user?.DisplayName ?? User.Identity?.Name;
            ViewBag.UploadCount = await _bookService.GetBookCountByUserAsync(userId);
            ViewBag.WishlistCount = await _wishlistService.GetWishlistCountByUserAsync(userId);
            ViewBag.ReviewCount = await _reviewService.GetReviewCountByUserAsync(userId);

            // Activity feed data
            ViewBag.RecentUploads = await _bookService.GetRecentUserBooksAsync(userId, 3);
            ViewBag.RecentWishlistItems = await _wishlistService.GetRecentWishlistItemsAsync(userId, 3);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                return statusCode.Value switch
                {
                    404 => View("Error404"),
                    500 => View("Error500"),
                    _ => View("Error500")
                };
            }

            return View("Error500");
        }

        [Route("/Error404")]
        public IActionResult Error404()
        {
            Response.StatusCode = 404;
            return View();
        }

        [Route("/Error500")]
        public IActionResult Error500()
        {
            Response.StatusCode = 500;
            return View();
        }
    }
}