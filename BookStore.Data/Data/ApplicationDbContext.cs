using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookStore.Data.Models;
using BookStore.Data.Models.Identity;

namespace BookStore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<Genre> Genres { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Wishlist> Wishlists { get; set; }
        public virtual DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Book-Genre many-to-many relationship
            builder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity(j => j.ToTable("BookGenres"));

            // Review relationships
            builder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Review>()
                .HasOne(r => r.PostedBy)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.PostedById)
                .OnDelete(DeleteBehavior.Cascade);

            // Wishlist relationships
            builder.Entity<Wishlist>()
                .HasOne(w => w.Book)
                .WithMany(b => b.Wishlists)
                .HasForeignKey(w => w.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlist)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Rating relationships
            builder.Entity<Rating>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Ratings)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint for user ratings (one rating per book per user)
            builder.Entity<Rating>()
                .HasIndex(r => new { r.UserId, r.BookId })
                .IsUnique();

            // Seed Genres
            builder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Fiction", Description = "Literary works based on imagination" },
                new Genre { Id = 2, Name = "Non-Fiction", Description = "Informational works based on facts" },
                new Genre { Id = 3, Name = "Science Fiction", Description = "Speculative fiction dealing with futuristic concepts" },
                new Genre { Id = 4, Name = "Fantasy", Description = "Magic and supernatural elements" },
                new Genre { Id = 5, Name = "Mystery", Description = "Crime, detective, and suspense stories" },
                new Genre { Id = 6, Name = "Biography", Description = "Accounts of people's lives" },
                new Genre { Id = 7, Name = "History", Description = "Historical accounts and analysis" },
                new Genre { Id = 8, Name = "Romance", Description = "Love stories and relationships" },
                new Genre { Id = 9, Name = "Thriller", Description = "Suspenseful and exciting plots" },
                new Genre { Id = 10, Name = "Self-Help", Description = "Personal development and improvement" }
            );

            // Seed some sample books
            builder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "The Great Gatsby",
                    ISBN = "9780743273565",
                    Author = "F. Scott Fitzgerald",
                    Description = "A classic novel about the American Dream in the Jazz Age",
                    Price = 12.99m,
                    CoverImageUrl = "/images/books/great-gatsby.jpg",
                    PublicationYear = 1925,
                    Publisher = "Scribner",
                    PageCount = 180,
                    UploadedById = "1",
                    AddedDate = DateTime.UtcNow
                },
                new Book
                {
                    Id = 2,
                    Title = "To Kill a Mockingbird",
                    ISBN = "9780061120084",
                    Author = "Harper Lee",
                    Description = "A gripping story of racial injustice in the American South",
                    Price = 14.99m,
                    CoverImageUrl = "/images/books/to-kill-a-mockingbird.jpg",
                    PublicationYear = 1960,
                    Publisher = "J.B. Lippincott & Co.",
                    PageCount = 281,
                    UploadedById = "1",
                    AddedDate = DateTime.UtcNow
                }
            );

            // Seed Book-Genre relationships
            builder.Entity("BookGenres").HasData(
                new { BooksId = 1, GenresId = 1 }, // Great Gatsby - Fiction
                new { BooksId = 1, GenresId = 9 }, // Great Gatsby - Thriller
                new { BooksId = 2, GenresId = 1 }, // Mockingbird - Fiction
                new { BooksId = 2, GenresId = 5 }  // Mockingbird - Mystery
            );

            // Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "User", NormalizedName = "USER" },
                new IdentityRole { Id = "2", Name = "Administrator", NormalizedName = "ADMINISTRATOR" }
            );
        }
    }
}