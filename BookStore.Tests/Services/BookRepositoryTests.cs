using BookStore.Data;
using BookStore.Data.Models;
using BookStore.Data.Models.Identity;
using BookStore.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookStore.Tests.Repositories
{
    public class BookRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly BookRepository _repository;

        public BookRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _repository = new BookRepository(_context);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async Task SeedUserAndGenre()
        {
            // Add required user
            var user = new ApplicationUser
            {
                Id = "user1",
                UserName = "test@test.com",
                Email = "test@test.com",
                DisplayName = "Test User"
            };
            _context.Users.Add(user);

            // Add a genre
            _context.Genres.Add(new Genre { Id = 1, Name = "Fiction" });

            await _context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetByIdWithDetailsAsync_ShouldIncludeRelatedData()
        {
            // Arrange
            await SeedUserAndGenre();
            var genre = await _context.Genres.FirstAsync();
            var book = new Book
            {
                Title = "Test Book",
                Author = "Author",
                ISBN = "1234567890",
                Description = "Desc",
                Price = 10m,
                CoverImageUrl = "url",
                UploadedById = "user1",
                AddedDate = DateTime.UtcNow
            };
            book.Genres.Add(genre);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdWithDetailsAsync(book.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Book", result.Title);
            Assert.Single(result.Genres);
            Assert.Equal("user1", result.UploadedById);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnMatchingBooks()
        {
            // Arrange
            await SeedUserAndGenre();
            var book1 = new Book
            {
                Title = "Harry Potter",
                Author = "J.K. Rowling",
                ISBN = "111",
                Description = "Magic",
                Price = 10,
                CoverImageUrl = "url",
                UploadedById = "user1"
            };
            var book2 = new Book
            {
                Title = "Lord of the Rings",
                Author = "Tolkien",
                ISBN = "222",
                Description = "Epic",
                Price = 15,
                CoverImageUrl = "url",
                UploadedById = "user1"
            };
            _context.Books.AddRange(book1, book2);
            await _context.SaveChangesAsync();

            // Act
            var results = await _repository.SearchAsync("Harry");

            // Assert
            Assert.Single(results);
            Assert.Equal("Harry Potter", results.First().Title);
        }

        [Fact]
        public async Task GetMostPopularAsync_ShouldOrderByAverageRating()
        {
            // Arrange
            await SeedUserAndGenre();

            var book1 = new Book
            {
                Title = "Book1",
                Author = "A",
                ISBN = "1",
                Description = "D",
                Price = 10,
                CoverImageUrl = "url",
                UploadedById = "user1"
            };
            var book2 = new Book
            {
                Title = "Book2",
                Author = "B",
                ISBN = "2",
                Description = "D",
                Price = 10,
                CoverImageUrl = "url",
                UploadedById = "user1"
            };

            // Ratings require UserId
            book1.Ratings.Add(new Rating { Value = 5, UserId = "user1" });
            book1.Ratings.Add(new Rating { Value = 4, UserId = "user1" });
            book2.Ratings.Add(new Rating { Value = 2, UserId = "user1" });

            _context.Books.AddRange(book1, book2);
            await _context.SaveChangesAsync();

            // Act
            var popular = await _repository.GetMostPopularAsync(2);

            // Assert
            Assert.Equal(2, popular.Count());
            Assert.Equal("Book1", popular.First().Title); // Average 4.5 > 2
        }

        [Fact]
        public async Task GetByTagAsync_ShouldReturnBooksWithGenre()
        {
            // Arrange
            await SeedUserAndGenre();
            var genre = await _context.Genres.FirstAsync();
            var book = new Book
            {
                Title = "Genre Book",
                Author = "A",
                ISBN = "111",
                Description = "D",
                Price = 10,
                CoverImageUrl = "url",
                UploadedById = "user1"
            };
            book.Genres.Add(genre);
            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            // Act
            var results = await _repository.GetByTagAsync("Fiction");

            // Assert
            Assert.Single(results);
            Assert.Equal("Genre Book", results.First().Title);
        }
    }
}