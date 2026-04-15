using BookStore.Data.Models;
using BookStore.Data.Models.Identity;
using BookStore.Services;
using BookStore.Services.Interfaces;
using BookStore.Web.Models;
using BookStore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookStore.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IBookService _bookService;
        private readonly IWishlistService _wishlistService;
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IBookService bookService,
            IWishlistService wishlistService,
            IReviewService reviewService,
            IOrderService orderService,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _bookService = bookService;
            _wishlistService = wishlistService;
            _reviewService = reviewService;
            _orderService = orderService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userBooks = await _bookService.GetBooksByUserAsync(user.Id);
            var wishlist = await _wishlistService.GetUserWishlistAsync(user.Id);
            var reviews = await _reviewService.GetReviewsByUserAsync(user.Id);
            var purchasedBooks = await _orderService.GetPurchasedBooksAsync(user.Id);
            var purchasedCount = purchasedBooks.Count();

            var viewModel = new ProfileViewModel
            {
                User = user,
                UploadedBooks = userBooks,
                WishlistBooks = wishlist,
                PurchasedBooks = purchasedBooks,
                RecentReviews = reviews,
                UploadCount = userBooks.Count(),
                WishlistCount = wishlist.Count(),
                ReviewCount = reviews.Count(),
                PurchasedCount = purchasedCount
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MyBooks(int? pageNumber, string tab = "added")
        {
            const int pageSize = 12;
            var user = await _userManager.GetUserAsync(User);
            IQueryable<Book> query;
            if (tab == "purchased")
            {
                query = _orderService.GetPurchasedBooksQueryable(user.Id);
                ViewBag.Tab = "purchased";
            }
            else
            {
                query = _bookService.GetBooksByUserQueryable(user.Id);
                ViewBag.Tab = "added";
            }
            var paginatedBooks = await PaginatedList<Book>.CreateAsync(query, pageNumber ?? 1, pageSize);

            return View("/Views/Profile/MyBooks.cshtml", paginatedBooks);
        }

        [HttpGet]
        public async Task<IActionResult> Wishlist(int? pageNumber)
        {
            const int pageSize = 12;
            var user = await _userManager.GetUserAsync(User);

            var wishlistQuery = _wishlistService.GetUserWishlistQueryable(user.Id);

            var paginatedWishlist = await PaginatedList<Book>.CreateAsync(wishlistQuery, pageNumber ?? 1, pageSize);
            return View("/Views/Profile/Wishlist.cshtml", paginatedWishlist);
        }

        [HttpGet]
        public async Task<IActionResult> Reviews(int? pageNumber)
        {
            const int pageSize = 10;
            var user = await _userManager.GetUserAsync(User);

            var reviewsQuery = _reviewService.GetReviewsByUserQueryable(user.Id);

            var paginatedReviews = await PaginatedList<Review>.CreateAsync(reviewsQuery, pageNumber ?? 1, pageSize);
            return View("/Views/Profile/Reviews.cshtml", paginatedReviews);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(new EditProfileViewModel
            {
                DisplayName = user.DisplayName,
                Bio = user.Bio,
                FavoriteGenre = user.FavoriteGenre,
                BirthYear = user.BirthYear
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                user.DisplayName = model.DisplayName;
                user.Bio = model.Bio;
                user.FavoriteGenre = model.FavoriteGenre;
                user.BirthYear = model.BirthYear;

                if (model.ProfileImage != null && model.ProfileImage.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(model.ProfileImage.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError(nameof(model.ProfileImage), "Only JPG, PNG, and GIF files are allowed");
                        return View(model);
                    }

                    var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "profile-images");
                    var uniqueFileName = $"{user.Id}_{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsPath, uniqueFileName);

                    Directory.CreateDirectory(uploadsPath);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfileImage.CopyToAsync(stream);
                    }

                    user.ProfileImageUrl = $"/profile-images/{uniqueFileName}";
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Index");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", User.FindFirstValue(ClaimTypes.NameIdentifier));
                ModelState.AddModelError("", "An error occurred while updating your profile.");
                return View(model);
            }
        }
    }
}