using FilmLister.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmLister.Service
{
    public class OrderService
    {
        public SortResult<T> OrderObjects<T>(IEnumerable<T> films) where T : AbstractComparable<T>
        {
            return OrderObjects(films, new QuickSort<T>());
       }

        public SortResult<T> OrderObjects<T>(IEnumerable<T> objectsToOrder, ISortAlgorithm<T> sortAlgorithm) where T : AbstractComparable<T>
        {
            if (objectsToOrder == null)
            {
                throw new ArgumentNullException(nameof(objectsToOrder));
            }

            if (sortAlgorithm == null)
            {
                throw new ArgumentNullException(nameof(sortAlgorithm));
            }

            var lowerRanks = objectsToOrder.ToDictionary(k => k, v => new List<T>());
            foreach(var o in objectsToOrder)
            {
                foreach(var hO in o.HigherRankedObjects)
                {
                    lowerRanks[hO].Add(o);
                }
            }

            var results = objectsToOrder
                .OrderBy(o => o.HigherRankedObjects.Count)
                .ThenBy(o => lowerRanks[o].Count)
                .ToArray();
            T left = null;
            T right = null;
            bool completed = true;

            try
            {
                sortAlgorithm.Sort(results);
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
