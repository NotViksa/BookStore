using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Services
{
    public class ReviewServiceTests
    {
        private readonly Mock<IReviewRepository> _mockReviewRepo;
        private readonly Mock<ILogger<ReviewService>> _mockLogger;
        private readonly ReviewService _reviewService;

        public ReviewServiceTests()
        {
            _mockReviewRepo = new Mock<IReviewRepository>();
            _mockLogger = new Mock<ILogger<ReviewService>>();
            _reviewService = new ReviewService(_mockReviewRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddReviewAsync_ShouldReturnCreatedReview()
        {
            // Arrange
            string content = "Great book!";
            int rating = 5;
            int bookId = 1;
            string userId = "user1";

            Review capturedReview = null;
            _mockReviewRepo.Setup(r => r.AddAsync(It.IsAny<Review>()))
                .Callback<Review>(r => capturedReview = r)
                .ReturnsAsync((Review r) => r);

            // Act
            var result = await _reviewService.AddReviewAsync(content, rating, bookId, userId);

            // Assert
            Assert.Equal(content, result.Content);
            Assert.Equal(rating, result.Rating);
            Assert.Equal(bookId, result.BookId);
            Assert.Equal(userId, result.PostedById);
            _mockReviewRepo.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_Owner_ShouldSucceed()
        {
            // Arrange
            int reviewId = 1;
            string userId = "user1";
            var review = new Review { Id = reviewId, PostedById = userId };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
            _mockReviewRepo.Setup(r => r.DeleteAsync(review)).ReturnsAsync(true);

            // Act
            var result = await _reviewService.DeleteReviewAsync(reviewId, userId, false);

            // Assert
            Assert.True(result);
            _mockReviewRepo.Verify(r => r.DeleteAsync(review), Times.Once);
        }

        [Fact]
        public async Task DeleteReviewAsync_Admin_ShouldSucceedEvenIfNotOwner()
        {
            // Arrange
            int reviewId = 1;
            string ownerId = "user1";
            string adminId = "admin";
            var review = new Review { Id = reviewId, PostedById = ownerId };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
            _mockReviewRepo.Setup(r => r.DeleteAsync(review)).ReturnsAsync(true);

            // Act
            var result = await _reviewService.DeleteReviewAsync(reviewId, adminId, true);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteReviewAsync_NotOwnerAndNotAdmin_ShouldReturnFalse()
        {
            // Arrange
            int reviewId = 1;
            string ownerId = "user1";
            string otherUserId = "user2";
            var review = new Review { Id = reviewId, PostedById = ownerId };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);

            // Act
            var result = await _reviewService.DeleteReviewAsync(reviewId, otherUserId, false);

            // Assert
            Assert.False(result);
            _mockReviewRepo.Verify(r => r.DeleteAsync(It.IsAny<Review>()), Times.Never);
        }

        [Fact]
        public async Task EditReviewAsync_ValidOwner_ShouldUpdate()
        {
            // Arrange
            int reviewId = 1;
            string userId = "user1";
            var review = new Review { Id = reviewId, PostedById = userId, Content = "Old", Rating = 3 };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(reviewId)).ReturnsAsync(review);
            _mockReviewRepo.Setup(r => r.UpdateAsync(review)).ReturnsAsync(true);

            // Act
            var result = await _reviewService.EditReviewAsync(reviewId, userId, "New content", 5);

            // Assert
            Assert.True(result);
            Assert.Equal("New content", review.Content);
            Assert.Equal(5, review.Rating);
        }

        [Fact]
        public async Task GetAverageRatingForBookAsync_ShouldReturnCorrectAverage()
        {
            // Arrange
            int bookId = 1;
            var reviews = new List<Review>
            {
                new Review { Rating = 4 },
                new Review { Rating = 5 }
            };
            _mockReviewRepo.Setup(r => r.GetCommentsForImageAsync(bookId)).ReturnsAsync(reviews);

            // Act
            var avg = await _reviewService.GetAverageRatingForBookAsync(bookId);

            // Assert
            Assert.Equal(4.5, avg);
        }

        [Fact]
        public async Task GetAverageRatingForBookAsync_NoReviews_ReturnsZero()
        {
            // Arrange
            _mockReviewRepo.Setup(r => r.GetCommentsForImageAsync(1)).ReturnsAsync(new List<Review>());

            // Act
            var avg = await _reviewService.GetAverageRatingForBookAsync(1);

            // Assert
            Assert.Equal(0, avg);
        }

        [Fact]
        public async Task EditReviewAsync_NotOwner_ShouldReturnFalse()
        {
            var review = new Review { Id = 1, PostedById = "owner" };
            _mockReviewRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(review);
            var result = await _reviewService.EditReviewAsync(1, "other", "new", 5);
            Assert.False(result);
            _mockReviewRepo.Verify(r => r.UpdateAsync(It.IsAny<Review>()), Times.Never);
        }
    }
}