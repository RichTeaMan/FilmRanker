namespace FilmLister.WebUI.Models
{
    public class FilmList
    {
        public int Id { get; set; }
        public bool Completed { get; set; }

        /// <summary>
        /// Gets films sorted by popularity. First element is least popular. Doesn't include ignored films.
        /// </summary>
        public Film[] SortedFilms { get; set; }

        /// <summary>
        /// Gets films that have been ignored. For instance, films that have not been seen.
        /// </summary>
        public Film[] IgnoredFilms { get; set; }

        public Film ChoiceA { get; set; }
        public Film ChoiceB { get; set; }
    }
}
