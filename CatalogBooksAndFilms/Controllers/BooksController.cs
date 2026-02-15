using Microsoft.AspNetCore.Authorization;
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

        public BooksController(ApplicationDbContext context, IAuthorService authorService)
        {
            _context = context;
            _authorService = authorService;
        }

        // ===============================
        // READ (Everyone)
        // ===============================

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .ToListAsync();

            return View(books);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        // ===============================
        // CREATE (Admin only)
        // ===============================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Authors = await _authorService.GetAllAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Authors = await _authorService.GetAllAsync();
                ViewBag.Genres = await _context.Genres.ToListAsync();
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

        // ===============================
        // UPDATE (Admin only)
        // ===============================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            ViewBag.Authors = await _authorService.GetAllAsync();
            ViewBag.Genres = await _context.Genres.ToListAsync();

            var model = new BookEditViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Year = book.Year,
                GenreId = book.GenreId,
                SelectedAuthorIds = book.BookAuthors.Select(x => x.AuthorId).ToList()
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Authors = await _authorService.GetAllAsync();
                ViewBag.Genres = await _context.Genres.ToListAsync();
                return View(model);
            }

            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.Id == model.Id);

            if (book == null)
                return NotFound();

            book.Title = model.Title;
            book.Description = model.Description;
            book.Year = model.Year;
            book.GenreId = model.GenreId;

            // Refresh many-to-many:
            _context.BookAuthors.RemoveRange(book.BookAuthors);

            book.BookAuthors = model.SelectedAuthorIds
                .Select(authorId => new BookAuthor
                {
                    BookId = book.Id,
                    AuthorId = authorId
                })
                .ToList();

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===============================
        // DELETE (Admin only)
        // ===============================

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _context.Books
                .Include(b => b.Genre)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return View(book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            _context.BookAuthors.RemoveRange(book.BookAuthors);
            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
