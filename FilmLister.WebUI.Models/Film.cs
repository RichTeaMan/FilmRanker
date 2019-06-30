using System.ComponentModel.DataAnnotations;

namespace FilmLister.WebUI.Models
{
    public class Film
    {
        public int Id { get; set; }
        public int TmdbId { get; set; }
        public string Name { get; set; }
        public string ImdbId { get; set; }
        public string ImageUrl { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", NullDisplayText = "Unknown")]
        public long? Budget { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}", NullDisplayText = "Unknown")]
        public long? Revenue { get; set; }
        public double? VoteAverage { get; set; }
        public string Director { get; set; }
        public int? ReleaseYear { get; set; }
        public string[] LesserRankedFilmNames { get; set; }

        public string DisplayName
        {
            get
            {
                return ReleaseYear.HasValue ? $"{Name} ({ReleaseYear})" : $"{Name}";
            }
        }

        /// <summary>
        /// Gets the IMDB link, or an empty string if there is no IMDB ID.
        /// </summary>
        public string ImdbLink
        {
            get
            {
                string imdbLink = string.Empty;
                if (!string.IsNullOrWhiteSpace(ImdbId))
                {
                    imdbLink = $"https://www.imdb.com/title/{ImdbId}/";
                }
                return imdbLink;
            }
        }
    }
}
