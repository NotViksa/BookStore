# BookStore - ASP.NET Core MVC Application

BookStore is a community-driven web platform for discovering, sharing, and reviewing books. Users can browse a catalog, add their own books, rate and review titles, maintain a wishlist, and complete purchases.

## Project Overview

This project was developed as the final assignment for the **ASP.NET Advanced** course at SoftUni (February 2026). It builds upon the fundamentals of ASP.NET Core MVC, Entity Framework Core, Identity, and Razor Pages, implementing a fully functional e‑bookstore with user roles, data validation, and a responsive UI.

## Features & Requirements Coverage

| Requirement                 | Implementation |
|-----------------------------|----------------|
| **15+ web pages**           | 20+ views including home, book catalog, details, cart, checkout, profile management, and custom error pages. |
| **6+ entity models**        | 9 models: `Book`, `Genre`, `Review`, `Rating`, `Wishlist`, `CartItem`, `Order`, `OrderItem`, `ApplicationUser`. |
| **5+ controllers**          | 6 controllers: `Home`, `Book`, `Cart`, `Genre`, `Profile`, `Review`. |
| **Razor & partial views**   | Extensive use of sections, partials (`_BookCard`, `_LoginPartial`), and the Razor engine. |
| **Entity Framework Core**   | SQL Server database accessed via EF Core with migrations and seeding. |
| **Identity & Roles**        | ASP.NET Core Identity with `User` and `Administrator` roles. Admin user seeded automatically. |
| **Pagination**              | Generic `PaginatedList<T>` used in book listings, profile sections, and search results. |
| **Search & Filtering**      | Search by title/author/genre; filter by genre and price range. |
| **Data Seeding**            | Roles, admin account, sample genres, and sample books are seeded on startup. |
| **Validation & Security**   | Client‑side and server‑side validation; Anti‑Forgery tokens; SQL injection protection via EF; XSS prevention via Razor encoding. |
| **Custom Error Pages**      | 404 and 500 error pages with navigation suggestions. |
| **Responsive Design**       | Bootstrap 5 + custom CSS, mobile‑friendly layout. |
| **AJAX Enhancements**       | Wishlist toggle and rating submission use asynchronous requests for smooth UX. |
| **File Upload**             | Book covers and profile images can be uploaded with extension/size validation. |
| **Dependency Injection**    | All services and repositories are registered in the DI container. |

## Architecture & Layers

The solution follows a clean, layered architecture:

- **Data Layer** (`BookStore.Data`): Entity models, `ApplicationDbContext`, repository interfaces and implementations.
- **Service Layer** (`BookStore.Services`): Business logic encapsulated in services like `BookService`, `CartService`, `RatingService`, etc.
- **Web Layer** (`BookStore.Web`): Controllers, ViewModels, Razor views, and static assets.

## Technologies Used

- ASP.NET Core
- Entity Framework Core (SQL Server)
- ASP.NET Core Identity
- jQuery & AJAX
<<<<<<< HEAD
- Razor Pages & MVC
=======
- Razor Pages & MVC
>>>>>>> d0b8233bc65227a562de8e2e02f85a427311d219
