using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Services;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Services
{
    public class GenreServiceTests
    {
        private readonly Mock<IGenreRepository> _mockGenreRepo;
        private readonly Mock<ILogger<GenreService>> _mockLogger;
        private readonly GenreService _genreService;

        public GenreServiceTests()
        {
            _mockGenreRepo = new Mock<IGenreRepository>();
            _mockLogger = new Mock<ILogger<GenreService>>();
            _genreService = new GenreService(_mockGenreRepo.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateGenreAsync_NewGenre_ShouldAdd()
        {
            // Arrange
            string name = "Sci-Fi";
            string description = "Science fiction";
            _mockGenreRepo.Setup(r => r.GetByNameAsync(name)).ReturnsAsync((Genre?)null);
            _mockGenreRepo.Setup(r => r.AddAsync(It.IsAny<Genre>())).ReturnsAsync((Genre g) => g);

            // Act
            var result = await _genreService.CreateGenreAsync(name, description);

            // Assert
            Assert.Equal(name, result.Name);
            Assert.Equal(description, result.Description);
            _mockGenreRepo.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Once);
        }

        [Fact]
        public async Task CreateGenreAsync_ExistingGenre_ShouldReturnExisting()
        {
            // Arrange
            string name = "Fantasy";
            var existing = new Genre { Id = 1, Name = name };
            _mockGenreRepo.Setup(r => r.GetByNameAsync(name)).ReturnsAsync(existing);

            // Act
            var result = await _genreService.CreateGenreAsync(name);

            // Assert
            Assert.Equal(existing.Id, result.Id);
            _mockGenreRepo.Verify(r => r.AddAsync(It.IsAny<Genre>()), Times.Never);
        }

        [Fact]
        public async Task UpdateGenreAsync_ValidId_ShouldUpdateAndReturn()
        {
            // Arrange
            int id = 1;
            var genre = new Genre { Id = id, Name = "Old" };
            _mockGenreRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(genre);
            _mockGenreRepo.Setup(r => r.UpdateAsync(genre)).Returns(Task.CompletedTask);

            // Act
            var result = await _genreService.UpdateGenreAsync(id, "New", "New Desc");

            // Assert
            Assert.Equal("New", result.Name);
            Assert.Equal("New Desc", result.Description);
            _mockGenreRepo.Verify(r => r.UpdateAsync(genre), Times.Once);
        }

        [Fact]
        public async Task UpdateGenreAsync_InvalidId_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            _mockGenreRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Genre?)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _genreService.UpdateGenreAsync(999, "Name", "Desc"));
        }

        [Fact]
        public async Task DeleteGenreAsync_Existing_ShouldReturnTrue()
        {
            // Arrange
            int id = 1;
            var genre = new Genre { Id = id };
            _mockGenreRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(genre);
            _mockGenreRepo.Setup(r => r.DeleteAsync(genre)).Returns(Task.CompletedTask);

            // Act
            var result = await _genreService.DeleteGenreAsync(id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task DeleteGenreAsync_NotFound_ShouldReturnFalse()
        {
            // Arrange
            _mockGenreRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Genre?)null);

            // Act
            var result = await _genreService.DeleteGenreAsync(1);

            // Assert
            Assert.False(result);
        }
    }
}