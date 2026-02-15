using Humanizer.Localisation;
using System.ComponentModel.DataAnnotations;

namespace CatalogBooksAndFilms.Entities
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int Year { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public ICollection<BookAuthor> BookAuthors { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
