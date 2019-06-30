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

        public long? Budget { get; set; }

        public long? Revenue { get; set; }

        public double? VoteAverage { get; set; }

        public string Director { get; set; }

        public Film(int id, string name, int tmdbId, string imageUrl, string imdbId, int? releaseYear, long? budget, long? revenue, double? voteAverage, string director)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            TmdbId = tmdbId;
            ImageUrl = imageUrl ?? throw new ArgumentNullException(nameof(imageUrl));
            ImdbId = imdbId;
            ReleaseYear = releaseYear;
            Budget = budget;
            Revenue = revenue;
            VoteAverage = voteAverage;
            Director = director;
        }
    }
}
