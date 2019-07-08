using System;
using System.Collections.Generic;

namespace FilmLister.Domain
{
    public class OrderedFilm : AbstractComparable<OrderedFilm>
    {
        public int Id { get; }
        public Film Film { get; }
        public bool Ignore { get; }

        private List<OrderedFilm> _lesserRankedFilms = new List<OrderedFilm>();
        public IReadOnlyList<OrderedFilm> LesserRankedFilms
        {
            get { return _lesserRankedFilms.AsReadOnly(); }
        }

        public OrderedFilm(int id, bool ignore, Film film)
        {
            Id = id;
            Ignore = ignore;
            Film = film ?? throw new ArgumentNullException(nameof(film));
        }

        public void AddLesserRankedFilm(OrderedFilm lesserRanked)
        {
            _lesserRankedFilms.Add(lesserRanked);
        }

        public void AddLesserRankedFilms(IEnumerable<OrderedFilm> lesserRanked)
        {
            _lesserRankedFilms.AddRange(lesserRanked);
        }
    }
}
