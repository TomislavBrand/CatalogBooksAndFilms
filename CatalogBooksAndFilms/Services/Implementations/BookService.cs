using Microsoft.EntityFrameworkCore;
using CatalogBooksAndFilms.Data;
using CatalogBooksAndFilms.Entities;
using CatalogBooksAndFilms.Services.Interfaces;

namespace CatalogBooksAndFilms.Services.Implementations
{
    public class BookService : IBookService
    {
        private readonly ApplicationDbContext _context;

        public BookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Book>> GetAllAsync()
            => await _context.Books.ToListAsync();

        public async Task<Book> GetByIdAsync(int id)
            => await _context.Books.FindAsync(id);

        public async Task AddAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
        }
    }
}