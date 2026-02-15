using CatalogBooksAndFilms.Entities;

namespace CatalogBooksAndFilms.Services.Interfaces
{
    public interface IAuthorService
    {
        Task<List<Author>> GetAllAsync();
    }
}
