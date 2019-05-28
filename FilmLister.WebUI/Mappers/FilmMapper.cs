using FilmLister.WebUI.Models;
namespace FilmLister.WebUI.Mappers
{
    public class FilmMapper
    {
        public Film Map(Domain.OrderedFilm orderedFilm)
        {
            Film film = null;
            if (orderedFilm != null)
            {
                film = new Film()
                {
                    Id = orderedFilm.Id,
                    Name = orderedFilm.Film.Name,
                    ImageUrl = orderedFilm.Film.ImageUrl,
                    ImdbId = orderedFilm.Film.ImdbId,
                    ReleaseYear = orderedFilm.Film.ReleaseYear
                };
            }
            return film;
        }

        public Film Map(Domain.Film film)
        {
            Film filmModel = null;
            if (film != null)
            {
                filmModel = new Film()
                {
                    Id = film.Id,
                    Name = film.Name,
                    ImageUrl = film.ImageUrl,
                    ImdbId = film.ImdbId,
                    ReleaseYear = film.ReleaseYear
                };
            }
            return filmModel;
        }
    }
}
