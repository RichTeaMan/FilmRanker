using System;

namespace FilmLister.Service.Exceptions
{
    public class FilmAlreadyInFilmListTemplateException : Exception
    {
        public int FilmId { get; }

        public int FilmListTemplateId { get; }

        public FilmAlreadyInFilmListTemplateException(int filmId, int filmListTemplateId)
        {
            FilmId = filmId;
            FilmListTemplateId = filmListTemplateId;
        }

        public override string Message => "The same film cannot be added to a list template multiple times.";
    }
}
