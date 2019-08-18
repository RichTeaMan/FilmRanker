using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmLister.Domain
{
    public class OrderedFilmRank
    {
        public int Id { get; }
        public bool Completed { get; }
        public IReadOnlyList<OrderedFilm> SortedFilms { get; }
        public IReadOnlyList<OrderedFilm> IgnoredFilms { get; }
        public OrderedFilm ChoiceA { get; }
        public OrderedFilm ChoiceB { get; }

        /// <summary>
        /// Gets the choices remaining. Null if it is uncertain.
        /// </summary>
        public int? ChoicesRemaining { get; }

        public OrderedFilmRank(
            int id,
            bool completed,
            IEnumerable<OrderedFilm> sortedFilms,
            IEnumerable<OrderedFilm> ignoredFilms,
            OrderedFilm choiceA,
            OrderedFilm choiceB,
            int? choicesRemaining)
        {
            Id = id;
            Completed = completed;
            SortedFilms = sortedFilms.ToList() ?? throw new ArgumentNullException(nameof(sortedFilms));
            IgnoredFilms = ignoredFilms.ToList() ?? throw new ArgumentNullException(nameof(ignoredFilms));
            ChoiceA = choiceA;
            ChoiceB = choiceB;
            ChoicesRemaining = choicesRemaining;
        }
    }
}
