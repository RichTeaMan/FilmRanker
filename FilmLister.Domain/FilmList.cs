using System;

namespace FilmLister.Domain
{
    public class FilmList
    {
        public int Id { get; }
        public string Name { get; }
        public Film[] Films { get; }
        public bool Published { get; }

        public FilmList(int id, string name, Film[] films, bool published)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Films = films ?? throw new ArgumentNullException(nameof(films));
            Published = published;
        }
    }
}
