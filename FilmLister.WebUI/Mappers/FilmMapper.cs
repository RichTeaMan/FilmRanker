using FilmLister.Domain;
using FilmLister.WebUI.Models;
namespace FilmLister.WebUI.Mappers
{
    public class FilmMapper
    {
        public Film Map(OrderedFilm orderedFilm)
        {
            Film film = null;
            if (orderedFilm != null)
            {
                film = new Film()
                {
                    Id = orderedFilm.Id,
                    Name = orderedFilm.Name
                };
            }
            return film;
        }
    }
}
