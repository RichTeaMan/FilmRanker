using FilmLister.Domain;
using FilmLister.WebUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Mappers
{
    public class FilmRankMapper
    {
        private readonly FilmMapper filmMapper;

        public FilmRankMapper(FilmMapper filmMapper)
        {
            this.filmMapper = filmMapper ?? throw new ArgumentNullException(nameof(filmMapper));
        }

        public FilmRank Map(OrderedFilmRank orderedFilmList)
        {
            var filmList = new FilmRank()
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
