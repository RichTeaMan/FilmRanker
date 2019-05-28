using System;

namespace FilmLister.Service.Exceptions
{
    public class FilmNotFoundException : Exception
    {
        public int FilmId { get; }

        public FilmNotFoundException(int filmId)
        {
            FilmId = filmId;
        }
    }
}
