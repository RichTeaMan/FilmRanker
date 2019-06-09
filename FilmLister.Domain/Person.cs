using System;
using System.Collections.Generic;
using System.Text;

namespace FilmLister.Domain
{
    public class Person
    {
        public int TmdbId { get; }

        public string Name { get; }

        public Person(int tmdbId, string name)
        {
            TmdbId = tmdbId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}
