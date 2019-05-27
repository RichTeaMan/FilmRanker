using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FilmLister.Persistence
{
    public class FilmListTemplate
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public List<FilmListItem> FilmListItems { get; set; }
    }
}
