using Microsoft.AspNetCore.Mvc;
using BookStore.Services.Interfaces;
using BookStore.Web.ViewModels;

namespace BookStore.Web.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IBookService _bookService;
        private readonly ILogger<GenreController> _logger;

        public GenreController(
            IGenreService genreService,
            IBookService bookService,
            ILogger<GenreController> logger)
        {
            _genreService = genreService;
            _bookService = bookService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var genres = await _genreService.GetAllGenresAsync();
            return View(genres);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string genreName, int? pageNumber)
        {
            if (string.IsNullOrWhiteSpace(genreName))
            {
                return NotFound();
            }

            var genre = await _genreService.GetGenreByNameAsync(genreName);
            if (genre == null)
            {
                return NotFound();
            }

            var books = await _bookService.GetBooksByGenreAsync(genreName);

            var viewModel = new GenreDetailsViewModel
            {
                Genre = genre,
                Books = books,
                BookCount = books.Count()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Popular()
        {
            var popularGenres = await _genreService.GetPopularGenresAsync(20);
            return View(popularGenres);
        }
    }
}