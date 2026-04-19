using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Services
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepository _ratingRepository;
        private readonly ILogger<RatingService> _logger;

        public RatingService(
            IRatingRepository ratingRepository,
            ILogger<RatingService> logger)
        {
            _ratingRepository = ratingRepository;
            _logger = logger;
        }

        public async Task<(bool Success, double AverageRating)> SubmitRatingAsync(string userId, int bookId, int ratingValue)
        {
            try
            {
                var existingRating = await _ratingRepository.GetVoteAsync(userId, bookId);

                if (existingRating != null)
                {
                    if (existingRating.Value == ratingValue)
                    {
                        await _ratingRepository.RemoveVoteAsync(existingRating);
                    }
                    else
                    {
                        existingRating.Value = ratingValue;
                        existingRating.RatedDate = DateTime.UtcNow;
                        await _ratingRepository.UpdateVoteAsync(existingRating);
                    }
                }
                else
                {
                    var rating = new Rating
                    {
                        UserId = userId,
                        BookId = bookId,
                        Value = ratingValue,
                        RatedDate = DateTime.UtcNow
                    };
                    await _ratingRepository.AddVoteAsync(rating);
                }

                var averageRating = await GetAverageRatingForBookAsync(bookId);
                return (true, averageRating);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting rating for user {UserId} and book {BookId}", userId, bookId);
                return (false, 0);
            }
        }

        public async Task<bool> RemoveRatingAsync(string userId, int bookId)
        {
            try
            {
                var rating = await _ratingRepository.GetVoteAsync(userId, bookId);
                if (rating != null)
                {
                    await _ratingRepository.RemoveVoteAsync(rating);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing rating for user {UserId} and book {BookId}", userId, bookId);
                return false;
            }
        }

        public async Task<double> GetAverageRatingForBookAsync(int bookId)
        {
            return await _ratingRepository.GetImageScoreAsync(bookId);
        }

        public async Task<int?> GetUserRatingAsync(string userId, int bookId)
        {
            var rating = await _ratingRepository.GetVoteAsync(userId, bookId);
            return rating?.Value;
        }

        public async Task<Rating?> GetRatingAsync(string userId, int bookId)
        {
            return await _ratingRepository.GetVoteAsync(userId, bookId);
        }

        public async Task<Dictionary<int, int>> GetRatingDistributionAsync(int bookId)
        {
            var ratings = await _ratingRepository.GetVoteAsync(null, bookId);
            var distribution = new Dictionary<int, int>();

            for (int i = 1; i <= 5; i++)
            {
                distribution[i] = 0;
            }
            return distribution;
        }

        public async Task<int> GetTotalRatingsCountAsync(int bookId)
        {
            return 0;
        }
    }
}