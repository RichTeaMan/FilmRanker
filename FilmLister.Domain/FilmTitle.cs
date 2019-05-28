using System;

namespace FilmLister.Domain
{
    public class FilmTitle
    {
        public int TmdbId { get; }

        public string Title { get; }

        public FilmTitle(int tmdbId, string title)
        {
            TmdbId = tmdbId;
            Title = title ?? throw new ArgumentNullException(nameof(title));
        }
    }
}
