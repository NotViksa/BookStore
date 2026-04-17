using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Services
{
    public class RatingServiceTests
    {
        private readonly Mock<IRatingRepository> _mockRatingRepo;
        private readonly Mock<ILogger<RatingService>> _mockLogger;
        private readonly RatingService _ratingService;

        public RatingServiceTests()
        {
            _mockRatingRepo = new Mock<IRatingRepository>();
            _mockLogger = new Mock<ILogger<RatingService>>();
            _ratingService = new RatingService(_mockRatingRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task SubmitRatingAsync_NewRating_ShouldAddVote()
        {
            // Arrange
            string userId = "user1";
            int bookId = 1;
            int ratingValue = 4;

            _mockRatingRepo.Setup(r => r.GetVoteAsync(userId, bookId))
                .ReturnsAsync((Rating?)null);
            _mockRatingRepo.Setup(r => r.GetImageScoreAsync(bookId))
                .ReturnsAsync(4);

            // Act
            var (success, avg) = await _ratingService.SubmitRatingAsync(userId, bookId, ratingValue);

            // Assert
            Assert.True(success);
            Assert.Equal(4, avg);
            _mockRatingRepo.Verify(r => r.AddVoteAsync(It.Is<Rating>(
                r => r.UserId == userId && r.BookId == bookId && r.Value == ratingValue)), Times.Once);
        }

        [Fact]
        public async Task SubmitRatingAsync_ExistingSameRating_ShouldRemoveVote()
        {
            // Arrange
            string userId = "user1";
            int bookId = 1;
            var existing = new Rating { UserId = userId, BookId = bookId, Value = 4 };

            _mockRatingRepo.Setup(r => r.GetVoteAsync(userId, bookId))
                .ReturnsAsync(existing);
            _mockRatingRepo.Setup(r => r.GetImageScoreAsync(bookId))
                .ReturnsAsync(0);

            // Act
            var (success, avg) = await _ratingService.SubmitRatingAsync(userId, bookId, 4);

            // Assert
            Assert.True(success);
            Assert.Equal(0, avg);
            _mockRatingRepo.Verify(r => r.RemoveVoteAsync(existing), Times.Once);
            _mockRatingRepo.Verify(r => r.UpdateVoteAsync(It.IsAny<Rating>()), Times.Never);
        }

        [Fact]
        public async Task SubmitRatingAsync_ExistingDifferentRating_ShouldUpdateVote()
        {
            // Arrange
            string userId = "user1";
            int bookId = 1;
            var existing = new Rating { UserId = userId, BookId = bookId, Value = 3 };

            _mockRatingRepo.Setup(r => r.GetVoteAsync(userId, bookId))
                .ReturnsAsync(existing);
            _mockRatingRepo.Setup(r => r.GetImageScoreAsync(bookId))
                .ReturnsAsync(5);

            // Act
            var (success, avg) = await _ratingService.SubmitRatingAsync(userId, bookId, 5);

            // Assert
            Assert.True(success);
            Assert.Equal(5, avg);
            Assert.Equal(5, existing.Value);
            _mockRatingRepo.Verify(r => r.UpdateVoteAsync(existing), Times.Once);
        }

        [Fact]
        public async Task GetUserRatingAsync_ShouldReturnValue()
        {
            _mockRatingRepo.Setup(r => r.GetVoteAsync("u1", 1)).ReturnsAsync(new Rating { Value = 4 });
            var result = await _ratingService.GetUserRatingAsync("u1", 1);
            Assert.Equal(4, result);
        }
    }
}