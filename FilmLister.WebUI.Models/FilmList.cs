namespace FilmLister.WebUI.Models
{
    public class FilmList
    {
        public int Id { get; set; }
        public bool Completed { get; set; }

        /// <summary>
        /// Gets films sorted by popularity. First element is least popular.
        /// </summary>
        public Film[] SortedFilms { get; set; }
        public Film ChoiceA { get; set; }
        public Film ChoiceB { get; set; }
    }
}
