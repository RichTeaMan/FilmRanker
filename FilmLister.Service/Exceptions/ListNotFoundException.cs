using System;

namespace FilmLister.Service.Exceptions
{
    public class ListNotFoundException : Exception
    {
        public int ListId { get; }

        public ListNotFoundException(int listId)
        {
            ListId = listId;
        }
    }
}
