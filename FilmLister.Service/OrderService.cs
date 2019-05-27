using FilmLister.Domain;
using System.Collections.Generic;
using System.Linq;

namespace FilmLister.Service
{
    public class OrderService
    {
        public SortResult<T> OrderObjects<T>(IEnumerable<T> films) where T : AbstractComparable<T>
        {
            T[] results;
            T left = null;
            T right = null;
            bool completed = true;

            try
            {
                results = new List<T>(films).ToArray();
                var sorter = new QuickSort<T>();
                sorter.Sort(results);
            }
            catch (UnknownComparisonException<T> exception)
            {
                results = films.ToArray();
                left = exception.Left;
                right = exception.Right;
                completed = false;
            }
            var sortResult = new SortResult<T>(results, completed, left, right);
            return sortResult;
        }

        public SortResult<OrderedFilm> OrderFilms(IEnumerable<OrderedFilm> films)
        {
            var sortResult = OrderObjects(films);
            return sortResult;
        }
    }
}
