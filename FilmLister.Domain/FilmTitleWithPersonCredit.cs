namespace FilmLister.Domain
{
    public class FilmTitleWithPersonCredit : FilmTitle
    {

        public int PersonTmdbId { get; }

        public string JobTitle { get; }

        public FilmTitleWithPersonCredit(
            int tmdbId,
            string title,
            string imageUrl,
            int? releaseYear,
            int personTmdbId,
            string jobTitle)
            : base(tmdbId, title, imageUrl, releaseYear)
        {
            PersonTmdbId = personTmdbId;
            JobTitle = jobTitle ?? string.Empty;
        }
    }
}
