using System.ComponentModel.DataAnnotations;

namespace CatalogBooksAndFilms.ViewModels
{
    public class BookEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int Year { get; set; }

        public int GenreId { get; set; }

        public List<int> SelectedAuthorIds { get; set; } = new();
    }
}
