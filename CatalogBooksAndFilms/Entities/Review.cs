using System.ComponentModel.DataAnnotations;

namespace CatalogBooksAndFilms.Entities
{
    public class Review
    {
        public int Id { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }

        public int? BookId { get; set; }
        public Book Book { get; set; }

        public int? MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}