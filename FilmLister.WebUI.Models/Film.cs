namespace FilmLister.WebUI.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImdbId { get; set; }
        public string ImageUrl { get; set; }
        public int? ReleaseYear { get; set; }

        public string DisplayName
        {
            get
            {
                return ReleaseYear.HasValue ? $"{Name} ({ReleaseYear})" : $"{Name}";
            }
        }
    }
}
