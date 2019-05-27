using System;
using System.Linq;

namespace FilmLister.WebUI.Mappers
{
    public class FilmListTemplateMapper
    {
        private readonly FilmMapper filmMapper;

        public FilmListTemplateMapper(FilmMapper filmMapper)
        {
            this.filmMapper = filmMapper ?? throw new ArgumentNullException(nameof(filmMapper));
        }

        public Models.FilmListTemplate Map(Domain.FilmListTemplate filmListTemplate)
        {
            var model = new Models.FilmListTemplate
            {
                Id = filmListTemplate.Id,
                Name = filmListTemplate.Name,
                Films = filmListTemplate.Films.Select(f => filmMapper.Map(f)).ToArray()
            };
            return model;
        }
    }
}
