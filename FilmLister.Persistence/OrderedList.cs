using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FilmLister.Persistence
{
    public class OrderedList
    {
        [Key]
        public int Id { get; set; }

        public List<OrderedFilm> OrderedFilms { get; set; }

        public FilmListTemplate FilmListTemplate { get; set; }
    }
}
