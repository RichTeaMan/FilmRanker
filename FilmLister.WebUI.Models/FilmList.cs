using System.ComponentModel.DataAnnotations;

namespace FilmLister.WebUI.Models
{
    public class FilmList
    {
        public int Id { get; set; }

        [StringLength(Persistence.Constants.FILM_LIST_TEMPLATE_MAX_LENGTH, MinimumLength = 1)]
        public string Name { get; set; }
        public Film[] Films { get; set; }

        public bool Published { get; set; }
    }
}
