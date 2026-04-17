using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Data.Repositories;
using BookStore.Services;
using BookStore.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Services
{
    public class BookServiceTests
    {
        private readonly Mock<IBookRepository> _mockBookRepo;
        private readonly Mock<IGenreService> _mockGenreService;
        private readonly Mock<ILogger<BookService>> _mockLogger;
        private readonly BookService _bookService;

        public BookServiceTests()
        {
            _mockBookRepo = new Mock<IBookRepository>();
            _mockGenreService = new Mock<IGenreService>();
            _mockLogger = new Mock<ILogger<BookService>>();
            _bookService = new BookService(_mockBookRepo.Object, _mockGenreService.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task AddBookAsync_WithValidData_ShouldAddBookAndAssignGenres()
        {
            // Arrange
            var book = new Book
            {
                Title = "Test Book",
                Author = "Test Author",
                ISBN = "1234567890",
                Description = "Test description",
                Price = 19.99m
            };

            var genres = new[] { "Fiction", "Fantasy" };

            _mockGenreService.Setup(s => s.EnsureGenreExistsAsync("Fiction"))
                .ReturnsAsync(new Genre { Id = 1, Name = "Fiction" });
            _mockGenreService.Setup(s => s.EnsureGenreExistsAsync("Fantasy"))
                .ReturnsAsync(new Genre { Id = 2, Name = "Fantasy" });

            _mockBookRepo.Setup(r => r.AddAsync(It.IsAny<Book>()))
                .Callback<Book>(b => b.Id = 1)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.AddBookAsync(book, genres);

            // Assert
            Assert.Equal(1, result.Id);
            Assert.Equal(2, result.Genres.Count);
            _mockBookRepo.Verify(r => r.AddAsync(It.Is<Book>(b => b.Title == "Test Book")), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_ExistingBook_ShouldClearCollectionsAndDelete()
        {
            // Arrange
            int bookId = 1;
            var book = new Book
            {
                Id = bookId,
                Title = "To Delete",
                Reviews = new List<Review> { new Review() },
                Ratings = new List<Rating> { new Rating() },
                Wishlists = new List<Wishlist> { new Wishlist() }
            };

            _mockBookRepo.Setup(r => r.GetByIdWithDetailsAsync(bookId))
                .ReturnsAsync(book);
            _mockBookRepo.Setup(r => r.DeleteAsync(bookId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _bookService.DeleteBookAsync(bookId);

            // Assert
            Assert.True(result);
            Assert.Empty(book.Reviews);
            Assert.Empty(book.Ratings);
            Assert.Empty(book.Wishlists);
            _mockBookRepo.Verify(r => r.DeleteAsync(bookId), Times.Once);
        }

        [Fact]
        public async Task DeleteBookAsync_BookNotFound_ShouldReturnFalse()
        {
            // Arrange
            _mockBookRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<int>()))
                .ReturnsAsync((Book?)null);

            // Act
            var result = await _bookService.DeleteBookAsync(999);

            // Assert
            Assert.False(result);
            _mockBookRepo.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task SearchBooksAsync_WithValidTerm_ShouldReturnFilteredBooks()
        {
            // Arrange
            string searchTerm = "Harry";
            var expectedBooks = new List<Book>
            {
                new Book { Title = "Harry Potter 1", Author = "J.K. Rowling" },
                new Book { Title = "Harry Potter 2", Author = "J.K. Rowling" }
            };

            _mockBookRepo.Setup(r => r.SearchAsync(searchTerm))
                .ReturnsAsync(expectedBooks);

            // Act
            var result = await _bookService.SearchBooksAsync(searchTerm);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.Title.Contains("Harry"));
        }

        [Fact]
        public async Task GetBooksByUserAsync_ShouldReturnUserBooks()
        {
            // Arrange
            string userId = "user123";
            var books = new List<Book> { new Book { UploadedById = userId } };
            _mockBookRepo.Setup(r => r.GetByUserAsync(userId)).ReturnsAsync(books);

            // Act
            var result = await _bookService.GetBooksByUserAsync(userId);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetBookCountByUserAsync_ShouldReturnCorrectCount()
        {
            // Arrange
            string userId = "user123";
            _mockBookRepo.Setup(r => r.CountAsync(It.IsAny<Expression<Func<Book, bool>>>()))
                .ReturnsAsync(5);

            // Act
            var count = await _bookService.GetBookCountByUserAsync(userId);

            // Assert
            Assert.Equal(5, count);
        }

        [Fact]
        public async Task GetRecentUserBooksAsync_ShouldReturnLimitedBooks()
        {
            // Arrange
            string userId = "user123";
            var books = new List<Book>
            {
                new Book { Id = 1, UploadedById = userId },
                new Book { Id = 2, UploadedById = userId }
            };

            _mockBookRepo.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<Book, bool>>>(),
                It.IsAny<Func<IQueryable<Book>, IOrderedQueryable<Book>>>(),
                3))
                .ReturnsAsync(books);

            // Act
            var result = await _bookService.GetRecentUserBooksAsync(userId, 3);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBooksByGenreAsync_ShouldReturnBooksWithMatchingGenre()
        {
            // Arrange
            string genreName = "Fiction";
            var books = new List<Book>
            {
                new Book { Title = "Book1" },
                new Book { Title = "Book2" }
            };

            _mockBookRepo.Setup(r => r.GetByTagAsync(genreName))
                .ReturnsAsync(books);

            // Act
            var result = await _bookService.GetBooksByGenreAsync(genreName);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBooksByAuthorAsync_ShouldReturnAuthorBooks()
        {
            // Arrange
            string author = "Tolkien";
            var books = new List<Book> { new Book { Author = author } };
            _mockBookRepo.Setup(r => r.GetByAuthorAsync(author)).ReturnsAsync(books);

            // Act
            var result = await _bookService.GetBooksByAuthorAsync(author);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetBooksByPriceRangeAsync_ShouldReturnFilteredBooks()
        {
            // Arrange
            decimal min = 10m, max = 50m;
            var books = new List<Book> { new Book { Price = 25m } };
            _mockBookRepo.Setup(r => r.GetBooksByPriceRangeAsync(min, max)).ReturnsAsync(books);

            // Act
            var result = await _bookService.GetBooksByPriceRangeAsync(min, max);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetMostPopularBooksAsync_ShouldReturnTopBooks()
        {
            // Arrange
            var books = new List<Book> { new Book(), new Book() };
            _mockBookRepo.Setup(r => r.GetMostPopularAsync(5)).ReturnsAsync(books);

            // Act
            var result = await _bookService.GetMostPopularBooksAsync(5);

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetSearchQueryable_EmptyTerm_ShouldReturnAllBooks()
        {
            // Arrange
            var books = new List<Book>
            {
                new Book { Title = "Book1" },
                new Book { Title = "Book2" }
            }.AsQueryable();

            _mockBookRepo.Setup(r => r.GetQueryable()).Returns(books);

            // Act
            var result = _bookService.GetSearchQueryable("");

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetBooksByUserQueryable_ShouldReturnUserBooks()
        {
            // Arrange
            string userId = "user1";
            var books = new List<Book>
            {
                new Book { UploadedById = userId },
                new Book { UploadedById = "other" }
            }.AsQueryable();

            _mockBookRepo.Setup(r => r.GetQueryable()).Returns(books);

            // Act
            var result = _bookService.GetBooksByUserQueryable(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(userId, result.First().UploadedById);
        }
    }
}