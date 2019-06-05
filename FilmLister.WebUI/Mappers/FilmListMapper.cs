using FilmLister.Domain;
using FilmLister.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Mappers
{
    public class FilmListMapper
    {
        private readonly FilmMapper filmMapper;

        public FilmListMapper(FilmMapper filmMapper)
        {
            this.filmMapper = filmMapper ?? throw new ArgumentNullException(nameof(filmMapper));
        }

        public FilmList Map(OrderedFilmList orderedFilmList)
        {
            var filmList = new FilmList()
            {
                Id = orderedFilmList.Id,
                Completed = orderedFilmList.Completed,
                ChoiceA = filmMapper.Map(orderedFilmList.ChoiceA),
                ChoiceB = filmMapper.Map(orderedFilmList.ChoiceB),
                SortedFilms = orderedFilmList.SortedFilms.Select(f => filmMapper.Map(f)).ToArray(),
                IgnoredFilms = orderedFilmList.IgnoredFilms.Select(f => filmMapper.Map(f)).ToArray()
            };
            return filmList;
        }
    }
}
