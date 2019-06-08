namespace FilmLister.Domain
{
    public class FilmTitleWithPersonCredit : FilmTitle
    {

        public string PersonTitle { get; }

        public string JobTitle { get; }

        public FilmTitleWithPersonCredit(
            int tmdbId,
            string title,
            string imageUrl,
            int? releaseYear,
            string personTitle,
            string jobTitle)
            : base(tmdbId, title, imageUrl, releaseYear)
        {
            PersonTitle = personTitle ?? string.Empty;
            JobTitle = jobTitle ?? string.Empty;
        }
    }
}
