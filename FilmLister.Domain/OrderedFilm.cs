using System;

namespace FilmLister.Domain
{
    public class OrderedFilm : AbstractComparable<OrderedFilm>
    {
        public int Id { get; }
        public string Name { get; }

        public OrderedFilm(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
