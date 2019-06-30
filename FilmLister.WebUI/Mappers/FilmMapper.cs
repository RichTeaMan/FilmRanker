using FilmLister.WebUI.Models;
using System.Linq;

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
                    TmdbId = orderedFilm.Film.TmdbId,
                    ReleaseYear = orderedFilm.Film.ReleaseYear,
                    LesserRankedFilmNames = orderedFilm.LesserRankedFilms.Select(f => f.Film.Name).ToArray(),
                    Budget = orderedFilm.Film.Budget,
                    Revenue = orderedFilm.Film.Revenue,
                    Director = orderedFilm.Film.Director,
                    VoteAverage = orderedFilm.Film.VoteAverage
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
                    TmdbId = film.TmdbId,
                    ReleaseYear = film.ReleaseYear,
                    LesserRankedFilmNames = new string[0],
                    Budget = film.Budget,
                    Revenue = film.Revenue,
                    Director = film.Director,
                    VoteAverage = film.VoteAverage
                };
            }
            return filmModel;
        }
    }
}
