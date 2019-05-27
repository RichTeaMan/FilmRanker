namespace FilmLister.WebUI.Models
{
    public class FilmListTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Film[] Films { get; set; }
    }
}
