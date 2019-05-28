using System;

namespace FilmLister.Domain
{
    public class Film
    {
        public int Id { get; }

        public string Name { get; }

        public int TmdbId { get; }

        public string ImageUrl { get; }

        public string ImdbId { get; }

        public int? ReleaseYear { get; }

        public Film(int id, string name, int tmdbId, string imageUrl, string imdbId, int? releaseYear)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TmdbId = tmdbId;
            ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
            ImdbId = imdbId ?? throw new ArgumentNullException(nameof(imdbId));
            ReleaseYear = releaseYear;
        }
    }
}
