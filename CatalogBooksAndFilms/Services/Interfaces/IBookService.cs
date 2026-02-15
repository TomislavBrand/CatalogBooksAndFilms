using CatalogBooksAndFilms.Entities;

namespace CatalogBooksAndFilms.Services.Interfaces
{
    public interface IBookService
    {
        Task<List<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(int id);
        Task AddAsync(Book book);
    }
}