using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CatalogBooksAndFilms.Data;
using CatalogBooksAndFilms.Entities;
using CatalogBooksAndFilms.Services.Interfaces;
using CatalogBooksAndFilms.ViewModels;

namespace CatalogBooksAndFilms.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorService _authorService;

        public BooksController(
            ApplicationDbContext context,
            IAuthorService authorService)
        {
            _context = context;
            _authorService = authorService;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.BookAuthors)
                .ThenInclude(ba => ba.Author)
                .ToListAsync();

            return View(books);
        }

        // CREATE GET
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _authorService.GetAllAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View();
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(BookCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Authors = await _authorService.GetAllAsync();
                return View(model);
            }

            var book = new Book
            {
                Title = model.Title,
                Description = model.Description,
                Year = model.Year,
                GenreId = model.GenreId,
                BookAuthors = new List<BookAuthor>()
            };

            // MANY-TO-MANY INSERT
            foreach (var authorId in model.SelectedAuthorIds)
            {
                book.BookAuthors.Add(new BookAuthor
                {
                    AuthorId = authorId
                });
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}