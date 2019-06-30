using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FilmLister.Persistence
{
    public class OrderedList
    {
        [Key]
        public int Id { get; set; }

        public bool Completed { get; set; }

        public List<OrderedFilm> OrderedFilms { get; set; }

        public FilmListTemplate FilmListTemplate { get; set; }

        public DateTimeOffset StartDateTime { get; set; }

        public DateTimeOffset? CompletedDateTime { get; set; }
    }
}
