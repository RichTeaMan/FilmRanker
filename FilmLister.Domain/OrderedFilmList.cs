using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmLister.Domain
{
    public class OrderedFilmList
    {
        public int Id { get; }
        public bool Completed { get; }
        public IReadOnlyList<OrderedFilm> SortedFilms { get; }
        public OrderedFilm ChoiceA { get; }
        public OrderedFilm ChoiceB { get; }

        public OrderedFilmList(int id, bool completed, IEnumerable<OrderedFilm> sortedFilms, OrderedFilm choiceA, OrderedFilm choiceB)
        {
            Id = id;
            Completed = completed;
            SortedFilms = sortedFilms.ToList() ?? throw new ArgumentNullException(nameof(sortedFilms));
            ChoiceA = choiceA;
            ChoiceB = choiceB;
        }
    }
}
