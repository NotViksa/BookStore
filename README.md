# BookStore

BookStore is a community-driven web platform for discovering, sharing, and reviewing books. Users can browse a catalog, add their own books, rate and review titles, maintain a wishlist, and complete purchases.

## Project Concept

The project was developed as the final assignment for the ASP.NET Advanced course at SoftUni (February 2026). It serves as a social cataloging platform where book enthusiasts can build a personal library, engage with community reviews, and manage their reading lists.

## Features and Requirements Compliance

| Requirement                     | Implementation                                                                                                          |
|---------------------------------|-------------------------------------------------------------------------------------------------------------------------|
| ASP.NET Core       | Built with .NET 8 using the minimal hosting model and Razor Pages.                                            |
| 15+ web pages (views)           | Distinct views including `Home`, `Wishlist`, `Reviews`, `Login`, `Register` `Browse`, `Book Details`, `Add Book`, `Cart`, `Checkout`, `Profile`, `Profile Management`, `Error 404 and 500 pages`, `Multiple Admin Views`... |
| 6+ entity models                | 8 models: `Book`, `Genre`, `Review`, `Rating`, `Wishlist`, `CartItem`, `Order`, `OrderItem`.          |
| 5+ controllers                  | 7 controllers: `Home`, `Book`, `Cart`, `Genre`, `Profile`, `Review`, `Pagination` plus an `AdminController` inside its own Area.            |
| Razor, sections, partial views  | `_Layout.cshtml`, `_BookCard.cshtml`, `_LoginPartial.cshtml` and `@section Scripts` are used throughout.                 |
| Entity Framework Core           | SQL Server accessed via EF Core with automatic migrations and seeding on startup.                                        |
| Identity with User & Admin roles| ASP.NET Core Identity. Roles `User` and `Administrator` are seeded automatically.                                       |
| MVC Areas                       | An `Admin` area separates administrative functions (User and Book management).                                          |
| Responsive design               | Bootstrap 5 plus custom CSS with a dark theme and orange accents which is also Mobile friendly.                       |
| AJAX                            | Wishlist toggle and rating submission use asynchronous requests for a smoother user experience.                         |
| Unit tests (65%+ coverage)      | Services and Repositories are tested with coverage exceeding the requirement.         |
| Error handling & validation     | Data annotations, client‑side validation, server‑side `ModelState` checks, global exception handler, and 404/500 pages. |
| Custom 404 and 500 pages        | `Error404.cshtml` and `Error500.cshtml` are displayed for missing resources and server errors.                           |
| Security                        | Anti‑Forgery tokens on all POST actions. Razor encodes output by default. EF Core prevents SQL injection.                |
| Dependency Injection            | All repositories and services are registered in the built‑in DI container.                                              |
| Pagination                      | `PaginatedList<T>` provides consistent pagination for book listings, wishlist, user books, and reviews.                 |
| Search and filtering            | Full‑text search by title, author, ISBN, and genre. Filter by genre and price range.                                    |
| Data seeding                    | Roles, an Administrator Account, Default Genres, and Two Sample Books are inserted when the database is created.        |
| Public deployment               | The application is hosted on Azure App Service with a public URL.                                                       |

## Architecture and Design Decisions

The solution is divided into three main layers:

- **Data Layer** (`BookStore.Data`): Contains entity models, `ApplicationDbContext`, and repository implementations.
- **Service Layer** (`BookStore.Services`): Houses business logic. Services such as `BookService`, `CartService`, and `RatingService` coordinate repository calls and enforce business rules.
- **Web Layer** (`BookStore.Web`): ASP.NET Core MVC application with controllers, view models, Razor views, and static assets. It references the service layer and never directly accesses the database.

**Key design choices:**

- Entity Framework Core is used with a code‑first approach. Migrations are applied automatically at startup.
- `PaginatedList<T>` provides reusable pagination logic.
- File uploads are validated by extension and size, and stored in `wwwroot` subfolders.
- Role constants are centralized in `RolesInitializer` to avoid magic strings.
- Admin area uses `[Area("Admin")]` and separate routing, keeping administrative features isolated.

## Usage Instructions

The application is publicly deployed and can be accessed at the following URL:

[https://bookstore-softuni-g4hyauhchhgcfxdp.canadacentral-01.azurewebsites.net](https://bookstore-softuni-g4hyauhchhgcfxdp.canadacentral-01.azurewebsites.net)

### User Accounts
- Register a new account using the **Register** page.
- Log in with an existing account.
- Administrator Account:  
  Email: `admin@bookstore.com`  
  Password: `AdminPassword123!`

### Browsing Books
- Visit the home page or click **Browse** in the navigation bar to view all books.
- Use the search bar to find books by title, author, or ISBN.
- Filter books by genre or price range using the left sidebar.

### Features for Authenticated Users
- Add new books to the catalog via **Add Book**.
- Rate books on a 1–5 star scale and write reviews.
- Toggle wishlist status on any book page.
- Manage your profile, uploaded books, wishlist, and reviews from the **Profile** menu.
- Add books to a shopping cart and complete a purchase with a fake (for demonstration) payment.

### Administrator Panel
- After logging in as an administrator, an **Admin** Page appears in the navigation bar.
- The admin dashboard shows site statistics.
- **Manage Users**: View all users, toggle administrator role, or delete users.
- **Manage Books**: View all books and delete any book (including those uploaded by other users).
- View the **Error500** page.

### Error Pages
- Visiting a non‑existent page displays a custom 404 error page.
- Unhandled server errors display a custom 500 error page.