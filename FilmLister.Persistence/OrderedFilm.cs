using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FilmLister.Persistence
{
    public class OrderedFilm
    {
        [Key]
        public int Id { get; set; }

        public Film Film { get; set; }

        public bool Ignored { get; set; }

        [InverseProperty("GreaterRankedFilm")]
        public List<OrderedFilmRankItem> GreaterRankedFilmItems { get; set; }

        [InverseProperty("LesserRankedFilm")]
        public List<OrderedFilmRankItem> LesserRankedFilmItems { get; set; }
    }
}
