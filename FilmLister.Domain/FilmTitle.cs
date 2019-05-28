using System;

namespace FilmLister.Domain
{
    public class FilmTitle
    {
        public int TmdbId { get; }

        public string Title { get; }

        public string ImageUrl { get; }

        public int? ReleaseYear { get; }

        public FilmTitle(int tmdbId, string title, string imageUrl, int? releaseYear)
        {
            TmdbId = tmdbId;
            Title = title ?? throw new ArgumentNullException(nameof(title));
            ImageUrl = imageUrl ?? string.Empty;
            ReleaseYear = releaseYear;
        }
    }
}
