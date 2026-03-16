using BookStore.Data.Models;
using BookStore.Data.Models.Identity;
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
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            UserManager<ApplicationUser> userManager,
            IBookService bookService,
            IWishlistService wishlistService,
            IReviewService reviewService,
            ILogger<ProfileController> logger)
        {
            _userManager = userManager;
            _bookService = bookService;
            _wishlistService = wishlistService;
            _reviewService = reviewService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userBooks = await _bookService.GetBooksByUserAsync(user.Id);
            var wishlist = await _wishlistService.GetUserWishlistAsync(user.Id);
            var reviews = await _reviewService.GetReviewCountByUserAsync(user.Id);

            var viewModel = new ProfileViewModel
            {
                User = user,
                UploadedBooks = userBooks,
                WishlistBooks = wishlist,
                ReviewCount = reviews,
                UploadCount = userBooks.Count(),
                WishlistCount = wishlist.Count()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MyBooks(int? pageNumber)
        {
            const int pageSize = 12;
            var user = await _userManager.GetUserAsync(User);
            var books = await _bookService.GetBooksByUserAsync(user.Id);

            var paginatedBooks = await PaginatedList<Book>.CreateAsync(
                books.AsQueryable(), pageNumber ?? 1, pageSize);

            return View(paginatedBooks);
        }

        [HttpGet]
        public async Task<IActionResult> Wishlist(int? pageNumber)
        {
            const int pageSize = 12;
            var user = await _userManager.GetUserAsync(User);
            var wishlist = await _wishlistService.GetUserWishlistAsync(user.Id);

            var paginatedWishlist = await PaginatedList<Book>.CreateAsync(
                wishlist.AsQueryable(), pageNumber ?? 1, pageSize);

            return View(paginatedWishlist);
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