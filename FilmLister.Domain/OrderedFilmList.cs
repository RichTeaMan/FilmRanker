namespace FilmLister.Domain
{
    public class OrderedFilmList
    {
        public string Id { get; set; }
        public bool Completed { get; set; }
        public OrderedFilm[] SortedFilms { get; set; }
        public OrderedFilm ChoiceA { get; set; }
        public OrderedFilm ChoiceB { get; set; }
    }
}
