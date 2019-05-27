namespace FilmLister.WebUI.Models
{
    public class FilmList
    {
        public string Id { get; set; }
        public bool Completed { get; set; }
        public Film[] SortedFilms { get; set; }
        public Film ChoiceA { get; set; }
        public Film ChoiceB { get; set; }
    }
}
