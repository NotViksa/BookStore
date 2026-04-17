using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly ILogger<ReviewService> _logger;

        public ReviewService(
            IReviewRepository reviewRepository,
            ILogger<ReviewService> logger)
        {
            _reviewRepository = reviewRepository;
            _logger = logger;
        }

        public async Task<Review> AddReviewAsync(string content, int rating, int bookId, string userId)
        {
            var review = new Review
            {
                Content = content,
                Rating = rating,
                BookId = bookId,
                PostedById = userId,
                PostedDate = System.DateTime.UtcNow
            };

            return await _reviewRepository.AddAsync(review);
        }

        public async Task<bool> DeleteReviewAsync(int reviewId, string userId, bool isAdmin = false)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId);
                if (review == null) return false;

                if (review.PostedById == userId || isAdmin)
                {
                    return await _reviewRepository.DeleteAsync(review);
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review with ID {ReviewId}", reviewId);
                return false;
            }
        }

        public async Task<bool> EditReviewAsync(int reviewId, string userId, string content, int rating)
        {
            try
            {
                var review = await _reviewRepository.GetByIdAsync(reviewId);
                if (review == null || review.PostedById != userId) return false;

                review.Content = content;
                review.Rating = rating;
                return await _reviewRepository.UpdateAsync(review);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing review with ID {ReviewId}", reviewId);
                return false;
            }
        }

        public async Task<Review?> GetReviewByIdAsync(int reviewId)
        {
            return await _reviewRepository.GetByIdAsync(reviewId);
        }

        public async Task<IEnumerable<Review>> GetReviewsForBookAsync(int bookId)
        {
            return await _reviewRepository.GetCommentsForImageAsync(bookId);
        }

        public async Task<int> GetReviewCountByUserAsync(string userId)
        {
            return await _reviewRepository.GetCountByUserAsync(userId);
        }

        public async Task<double> GetAverageRatingForBookAsync(int bookId)
        {
            var reviews = await _reviewRepository.GetCommentsForImageAsync(bookId);
            if (!reviews.Any()) return 0;

            return reviews.Average(r => r.Rating);
        }
        public IQueryable<Review> GetReviewsByUserQueryable(string userId)
        {
            return _reviewRepository.GetReviewsByUserQueryable(userId);
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserAsync(string userId)
        {
            return await _reviewRepository.GetReviewsByUserAsync(userId);
        }
        public int GetTotalReviewsCount()
        {
            return _reviewRepository.GetTotalCount();
        }
    }
}