using System.ComponentModel.DataAnnotations;

namespace FilmLister.Persistence
{
    public class FilmListItem
    {
        [Key]
        public int Id { get; set; }

        public Film Film { get; set; }

        public FilmListTemplate FilmListTemplate { get; set; }
    }
}
