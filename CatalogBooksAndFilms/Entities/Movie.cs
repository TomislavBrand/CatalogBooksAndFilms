using Humanizer.Localisation;
using System.ComponentModel.DataAnnotations;

namespace CatalogBooksAndFilms.Entities
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int ReleaseYear { get; set; }

        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }
}