using FilmLister.Domain;

namespace FilmLister.Service
{
    public interface ISortAlgorithm<T> where T : AbstractComparable<T>
    {
        SortResult<T> Sort(T[] entries);
    }
}
