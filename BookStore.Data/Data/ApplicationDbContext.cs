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
        public virtual DbSet<CartItem> CartItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Book-Genre many-to-many
            builder.Entity<Book>()
                .HasMany(b => b.Genres)
                .WithMany(g => g.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookGenres",
                    j => j.HasOne<Genre>().WithMany().HasForeignKey("GenreId").OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<Book>().WithMany().HasForeignKey("BookId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasKey("BookId", "GenreId"));

            // Review relationships
            builder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.PostedBy)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.PostedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Wishlist relationships
            builder.Entity<Wishlist>()
                .HasOne(w => w.Book)
                .WithMany(b => b.Wishlists)
                .HasForeignKey(w => w.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.Wishlist)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Rating relationships
            builder.Entity<Rating>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Ratings)
                .HasForeignKey(r => r.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany(u => u.Ratings)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraint: one rating per user per book
            builder.Entity<Rating>()
                .HasIndex(r => new { r.UserId, r.BookId })
                .IsUnique();

            // CartItem
            builder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CartItem>()
                .HasOne(c => c.Book)
                .WithMany()
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            // Order
            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem
            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany()
                .HasForeignKey(oi => oi.BookId)
                .OnDelete(DeleteBehavior.Restrict);

            //
            // Genres (no dependencies, always safe)
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

            // Seed Roles
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "User", NormalizedName = "USER" },
                new IdentityRole { Id = "2", Name = "Administrator", NormalizedName = "ADMINISTRATOR" }
            );


        }
    }
}