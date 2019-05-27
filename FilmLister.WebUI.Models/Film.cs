using System;

namespace FilmLister.WebUI.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImdbId { get; set; }
        public string ImageUrl { get; set; }
    }
}
