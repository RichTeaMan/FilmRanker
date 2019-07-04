using System;
using System.Linq;

namespace FilmLister.WebUI.Mappers
{
    public class FilmListMapper
    {
        private readonly FilmMapper filmMapper;

        public FilmListMapper(FilmMapper filmMapper)
        {
            this.filmMapper = filmMapper ?? throw new ArgumentNullException(nameof(filmMapper));
        }

        public Models.FilmList Map(Domain.FilmList filmListTemplate)
        {
            var model = new Models.FilmList
            {
                Id = filmListTemplate.Id,
                Name = filmListTemplate.Name,
                Films = filmListTemplate.Films.Select(f => filmMapper.Map(f)).ToArray(),
                Published = filmListTemplate.Published
            };
            return model;
        }
    }
}
