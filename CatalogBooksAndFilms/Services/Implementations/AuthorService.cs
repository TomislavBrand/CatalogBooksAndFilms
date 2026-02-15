using Microsoft.EntityFrameworkCore;
using CatalogBooksAndFilms.Data;
using CatalogBooksAndFilms.Entities;
using CatalogBooksAndFilms.Services.Interfaces;

namespace CatalogBooksAndFilms.Services.Implementations
{
    public class AuthorService : IAuthorService
    {
        private readonly ApplicationDbContext _context;

        public AuthorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Author>> GetAllAsync()
            => await _context.Authors.ToListAsync();
    }
}