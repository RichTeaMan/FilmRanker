using System.ComponentModel.DataAnnotations;

namespace FilmLister.Persistence
{
    public class OrderedFilmRankItem
    {
        [Key]
        public int Id { get; set; }
        public OrderedFilm LesserRankedFilm { get; set; }

        public OrderedFilm GreaterRankedFilm { get; set; }
    }
}
