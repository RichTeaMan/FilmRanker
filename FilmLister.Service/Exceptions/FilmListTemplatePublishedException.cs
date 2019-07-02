using System;

namespace FilmLister.Service.Exceptions
{
    public class FilmListTemplatePublishedException : Exception
    {
        public int FilmListTemplateId { get; }

        public override string Message => "A film list template cannot be modified once it has been published.";

        public FilmListTemplatePublishedException(int filmListTemplateId)
        {
            FilmListTemplateId = filmListTemplateId;
        }

    }
}
