using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using BookStore.Web.Models;
using BookStore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;

namespace BookStore.Web.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookService _bookService;
        private readonly IGenreService _genreService;
        private readonly IWishlistService _wishlistService;
        private readonly IRatingService _ratingService;
        private readonly IReviewService _reviewService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<BookController> _logger;

        public BookController(
            IBookService bookService,
            IGenreService genreService,
            IWishlistService wishlistService,
            IRatingService ratingService,
            IReviewService reviewService,
            IWebHostEnvironment environment,
            ILogger<BookController> logger)
        {
            _bookService = bookService;
            _genreService = genreService;
            _wishlistService = wishlistService;
            _ratingService = ratingService;
            _reviewService = reviewService;
            _environment = environment;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? genre, string? author, decimal? minPrice, decimal? maxPrice, int? pageNumber)
        {
            const int pageSize = 12;
            var query = _bookService.GetBooksQueryable();
            query = query.Include(b => b.Ratings);

            // Apply filters
            if (!string.IsNullOrEmpty(genre))
            {
                query = query.Where(b => b.Genres.Any(g => g.Name == genre));
                ViewBag.CurrentGenre = genre;
            }

            if (!string.IsNullOrEmpty(author))
            {
                query = query.Where(b => b.Author.Contains(author));
                ViewBag.CurrentAuthor = author;
            }

            if (minPrice.HasValue)
            {
                query = query.Where(b => b.Price >= minPrice.Value);
                ViewBag.MinPrice = minPrice;
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= maxPrice.Value);
                ViewBag.MaxPrice = maxPrice;
            }

            var popularGenres = await _genreService.GetPopularGenresAsync(10);
            ViewBag.PopularGenres = popularGenres;

            var paginatedBooks = await PaginatedList<Book>.CreateAsync(query, pageNumber ?? 1, pageSize);
            return View(paginatedBooks);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            var reviews = await _reviewService.GetReviewsForBookAsync(id);
            var averageRating = await _ratingService.GetAverageRatingForBookAsync(id);

            var viewModel = new BookDetailsViewModel
            {
                Book = book,
                Reviews = reviews,
                AverageRating = averageRating
            };

            if (User.Identity.IsAuthenticated)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewBag.IsInWishlist = await _wishlistService.IsInWishlistAsync(userId, id);
                ViewBag.UserRating = await _ratingService.GetUserRatingAsync(userId, id);
            }

            return View(viewModel);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AddBook()
        {
            return View(new AddBookViewModel());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook(AddBookViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Validate file
            if (model.CoverImage == null || model.CoverImage.Length == 0)
            {
                ModelState.AddModelError(nameof(model.CoverImage), "Please select a cover image");
                return View(model);
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(model.CoverImage.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError(nameof(model.CoverImage), "Only JPG, PNG, and GIF files are allowed");
                return View(model);
            }

            // Validate file size (max 5MB)
            if (model.CoverImage.Length > 5 * 1024 * 1024)
            {
                ModelState.AddModelError(nameof(model.CoverImage), "File size cannot exceed 5MB");
                return View(model);
            }

            try
            {
                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_environment.WebRootPath, "book-covers");
                Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsPath, uniqueFileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }

                var book = new Book
                {
                    Title = model.Title,
                    Author = model.Author,
                    ISBN = model.ISBN,
                    Description = model.Description,
                    Price = model.Price,
                    PublicationYear = model.PublicationYear,
                    Publisher = model.Publisher,
                    PageCount = model.PageCount,
                    CoverImageUrl = $"/book-covers/{uniqueFileName}",
                    UploadedById = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    AddedDate = DateTime.UtcNow
                };

                var genreList = model.Genres?.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim())
                    ?? Array.Empty<string>();

                await _bookService.AddBookAsync(book, genreList);

                TempData["SuccessMessage"] = "Book added successfully!";
                return RedirectToAction("Details", new { id = book.Id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding book");
                ModelState.AddModelError("", "An error occurred while adding the book. Please try again.");
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleWishlist(int bookId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isNowInWishlist = await _wishlistService.ToggleWishlistAsync(userId, bookId);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, isInWishlist = isNowInWishlist });
                }

                return RedirectToAction("Details", new { id = bookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling wishlist");
                return Json(new { success = false, error = "An error occurred" });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRating(int bookId, int ratingValue)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var (success, averageRating) = await _ratingService.SubmitRatingAsync(userId, bookId, ratingValue);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success, averageRating });
                }

                return RedirectToAction("Details", new { id = bookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting rating");
                return Json(new { success = false, error = "An error occurred" });
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            // Check if current user is owner or admin
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (book.UploadedById != currentUserId && !User.IsInRole("Administrator"))
            {
                return Forbid();
            }

            var success = await _bookService.DeleteBookAsync(id);
            if (!success)
            {
                return NotFound();
            }

            TempData["SuccessMessage"] = "Book deleted successfully!";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query, int? pageNumber)
        {
            const int pageSize = 12;

            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction("Index");
            }

            var results = _bookService.GetSearchQueryable(query);
            ViewBag.SearchQuery = query;

            var paginatedResults = await PaginatedList<Book>.CreateAsync(results, pageNumber ?? 1, pageSize);
            return View("Index", paginatedResults);
        }

        [HttpGet]
        public IActionResult ByAuthor(string author)
        {
            if (string.IsNullOrWhiteSpace(author))
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", new { author = author });
        }
    }
}