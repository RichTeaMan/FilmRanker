using FilmLister.Domain;
using System;
using System.Collections.Generic;

namespace FilmLister.Service
{
    public class OrderService
    {
        public SortResult<T> OrderObjects<T>(IEnumerable<T> films) where T : AbstractComparable<T>
        {
            T[] results;
            results = new List<T>(films).ToArray();
            T left = null;
            T right = null;
            bool completed = true;

            try
            {
                var sorter = new QuickSort<T>();
                // var sorter = new MergeSort<T>();
                sorter.Sort(results);
            }
            catch (UnknownComparisonException<T> exception)
            {
                // TODO: Add check that result is the correct length
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
