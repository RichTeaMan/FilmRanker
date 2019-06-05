using System;

namespace FilmLister.Domain
{
    public class OrderedFilm : AbstractComparable<OrderedFilm>
    {
        public int Id { get; }
        public Film Film { get; }
        public bool Ignore { get; }

        public OrderedFilm(int id, bool ignore, Film film)
        {
            Id = id;
            Ignore = ignore;
            Film = film ?? throw new ArgumentNullException(nameof(film));
        }
    }
}
