using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PurrfectPics.Data.Models;
using PurrfectPics.Data.Models.Identity;

namespace PurrfectPics.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> CatImages { get; set; }
        public virtual DbSet<Genre> Tags { get; set; }
        public virtual DbSet<Review> Comments { get; set; }
        public virtual DbSet<Wishlist> Favorites { get; set; }
        public virtual DbSet<Rating> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Book>()
                .HasMany(ci => ci.Tags)
                .WithMany(t => t.CatImages)
                .UsingEntity(j => j.ToTable("CatImageTags"));

            builder.Entity<Review>()
                .HasOne(c => c.CatImage)
                .WithMany(ci => ci.Comments)
                .HasForeignKey(c => c.CatImageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rating>()
                .HasOne(v => v.CatImage)
                .WithMany(ci => ci.Votes)
                .HasForeignKey(v => v.CatImageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Wishlist>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Rating>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);


            // Roles

            builder.Entity<IdentityRole>().HasData(
        new IdentityRole { Id = "1", Name = "User", NormalizedName = "USER" },
        new IdentityRole { Id = "2", Name = "Administrator", NormalizedName = "ADMINISTRATOR" }
    );

        }
    }
}