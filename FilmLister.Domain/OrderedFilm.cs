using System;

namespace FilmLister.Domain
{
    public class OrderedFilm : AbstractComparable<OrderedFilm>
    {
        public int Id { get; }
        public Film Film { get; }

        public OrderedFilm(int id, Film film)
        {
            Id = id;
            Film = film ?? throw new ArgumentNullException(nameof(film));
        }
    }
}
