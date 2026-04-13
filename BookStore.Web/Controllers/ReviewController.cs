using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;
using BookStore.Web.ViewModels;
using BookStore.Web.Models;
using System.Security.Claims;

namespace BookStore.Web.Controllers
{
    [Authorize]
    public class ReviewController : Controller
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(
            IReviewService reviewService,
            ILogger<ReviewController> logger)
        {
            _reviewService = reviewService;
            _logger = logger;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(AddReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide valid review content and rating.";
                return RedirectToAction("Details", "Book", new { id = model.BookId });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var review = await _reviewService.AddReviewAsync(
                    model.Content,
                    model.Rating,
                    model.BookId,
                    userId);

                TempData["SuccessMessage"] = "Review added successfully!";
                return RedirectToAction("Details", "Book", new { id = model.BookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding review");
                TempData["ErrorMessage"] = "Failed to add review. Please try again.";
                return RedirectToAction("Details", "Book", new { id = model.BookId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(int reviewId, int bookId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Administrator");

                var success = await _reviewService.DeleteReviewAsync(reviewId, userId, isAdmin);

                if (!success)
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete this review";
                }
                else
                {
                    TempData["SuccessMessage"] = "Review deleted successfully!";
                }

                return RedirectToAction("Details", "Book", new { id = bookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review");
                TempData["ErrorMessage"] = "An error occurred while deleting the review";
                return RedirectToAction("Details", "Book", new { id = bookId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReview(EditReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Details", "Book", new { id = model.BookId });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var success = await _reviewService.EditReviewAsync(
                    model.ReviewId,
                    userId,
                    model.Content,
                    model.Rating);

                if (!success)
                {
                    return NotFound();
                }

                TempData["SuccessMessage"] = "Review updated successfully!";
                return RedirectToAction("Details", "Book", new { id = model.BookId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing review");
                TempData["ErrorMessage"] = "An error occurred while editing the review";
                return RedirectToAction("Details", "Book", new { id = model.BookId });
            }
        }
    }
}