using CatalogBooksAndFilms.Entities;
using CatalogBooksAndFilms.Services.Implementations;
using Xunit;

namespace CatalogBooksAndFilms.Tests
{
    public class AuthorServiceTests
    {
        [Fact]
        public async Task GetAllAsync_ReturnsAllAuthors()
        {
            // Arrange
            using var db = TestDbFactory.CreateDbContext();

            db.Authors.AddRange(
                new Author { Name = "Author1" },
                new Author { Name = "Author2" }
            );

            await db.SaveChangesAsync();

            var service = new AuthorService(db);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count);
        }
    }
}
