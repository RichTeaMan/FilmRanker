using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FilmLister.Persistence
{
    public class Film
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int TmdbId { get; set; }

        public string ImageUrl { get; set; }

        public string ImdbId { get; set; }

        public long? Budget { get; set; }

        public long? Revenue { get; set; }

        public string Director { get; set; }

        public DateTimeOffset? ReleaseDate { get; set; }

        public List<FilmListItem> FilmListItems { get; set; }
    }
}
