using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FilmLister.Persistence
{
    public class OrderedFilm
    {
        [Key]
        public int Id { get; set; }

        public Film Film { get; set; }

        public List<Film> HigherRankedFilms { get; set; }

        
    }
}
