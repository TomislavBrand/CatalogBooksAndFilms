using System.ComponentModel.DataAnnotations;

namespace CatalogBooksAndFilms.ViewModels
{
    public class BookCreateViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int GenreId { get; set; }

        public int Year { get; set; }

        // Selected authors from UI
        public List<int> SelectedAuthorIds { get; set; } = new();
    }
}