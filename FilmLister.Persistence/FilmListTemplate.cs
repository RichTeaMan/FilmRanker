using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FilmLister.Persistence
{
    public class FilmListTemplate
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(Constants.FILM_LIST_TEMPLATE_MAX_LENGTH)]
        public string Name { get; set; }

        public List<FilmListItem> FilmListItems { get; set; }
    }
}
