using BookStore.Data;
using BookStore.Data.Interfaces;
using BookStore.Data.Models;
using BookStore.Data.Models.Identity;
using BookStore.Data.Repositories;
using BookStore.Services;
using BookStore.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration debug check
if (builder.Configuration.GetConnectionString("DefaultConnection") == null)
{
    Console.WriteLine("Warning: DefaultConnection not found in configuration sources");
    Console.WriteLine($"Configuration sources: {string.Join(", ", builder.Configuration.Sources.Select(s => s.ToString()))}");
}

// Get connection string with fallback
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=BookStore;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IGenreRepository, GenreRepository>();

// Register services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();
builder.Services.AddScoped<IRatingService, RatingService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Configure Identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdministratorRole",
        policy => policy.RequireRole("Administrator"));
    options.AddPolicy("RequireUserRole",
        policy => policy.RequireRole("User"));
});

builder.Services.AddControllersWithViews();

// Configure file upload limits
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 20 * 1024 * 1024; // 20MB
});

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 20 * 1024 * 1024; // 20MB
});

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");
app.UseExceptionHandler("/Home/Error");

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Initialize roles and admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        // Create roles if they don't exist
        string[] roleNames = { "Administrator", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create admin user if it doesn't exist
        var adminEmail = "admin@bookstore.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Admin",
                Bio = "Bookstore Administrator",
                FavoriteGenre = "Classics",
                BirthYear = 1990,
                RegistrationDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, "AdminPassword123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Administrator");
            }
        }

        // Seed some sample data if needed
        var bookRepository = services.GetRequiredService<IBookRepository>();
        if (await bookRepository.CountAsync(b => true) == 0)
        {
            // Add sample books programmatically if needed
            Console.WriteLine("Database seeded with sample books");
        }
        // Seed sample books (only if table is empty)
        var context = services.GetRequiredService<ApplicationDbContext>();
        if (!context.Books.Any())
        {
            adminUser = await userManager.FindByEmailAsync(adminEmail);
            var fiction = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Fiction");
            var thriller = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Thriller");
            var mystery = await context.Genres.FirstOrDefaultAsync(g => g.Name == "Mystery");

            var gatsby = new Book
            {
                Title = "The Great Gatsby",
                ISBN = "9780743273565",
                Author = "F. Scott Fitzgerald",
                Description = "A classic novel about the American Dream in the Jazz Age",
                Price = 12.99m,
                CoverImageUrl = "/images/books/great-gatsby.jpg",
                PublicationYear = 1925,
                Publisher = "Scribner",
                PageCount = 180,
                UploadedById = adminUser.Id,
                AddedDate = DateTime.UtcNow
            };
            if (fiction != null) gatsby.Genres.Add(fiction);
            if (thriller != null) gatsby.Genres.Add(thriller);
            context.Books.Add(gatsby);

            var mockingbird = new Book
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
                UploadedById = adminUser.Id,
                AddedDate = DateTime.UtcNow
            };
            if (fiction != null) mockingbird.Genres.Add(fiction);
            if (mystery != null) mockingbird.Genres.Add(mystery);
            context.Books.Add(mockingbird);

            await context.SaveChangesAsync();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.MapRazorPages();

app.MapControllerRoute(
    name: "errors",
    pattern: "Home/Error{statusCode}",
    defaults: new { controller = "Home", action = "Error" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapFallbackToController("Error404", "Home");

app.Run();