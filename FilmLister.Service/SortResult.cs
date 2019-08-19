using System;
using System.Collections.Generic;

namespace FilmLister.Service
{
    /// <summary>
    /// Contains results of a sort for objects with type <see cref="T"/>. If a pair of objects cannot be sorted they are presented in <see cref="LeftSort"/> and <see cref="RightSort"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SortResult<T>
    {
        /// <summary>
        /// Gets sorted results.
        /// </summary>
        /// <remarks>
        /// These results may not actually be sorted, check <see cref="Completed"/>.
        /// </remarks>
        public IReadOnlyList<T> SortedResults { get; }

        /// <summary>
        /// Gets if the sort is complete.
        /// </summary>
        public bool Completed { get; }

        /// <summary>
        /// Gets the 'left' objects that still needs to be sorted.
        /// </summary>
        /// <remarks>
        /// If the sort is not completed this member will an object that needs to be compared to <see cref="RightSort"/>.
        /// Otherwise this is null.
        /// </remarks>
        public T LeftSort { get; }

        /// <summary>
        /// Gets the 'right' objects that still needs to be sorted.
        /// </summary>
        /// <remarks>
        /// If the sort is not completed this member will an object that needs to be compared to <see cref="LeftSort"/>.
        /// Otherwise this is null.
        /// </remarks>
        public T RightSort { get; }

        [Obsolete]
        public SortResult(IReadOnlyList<T> sortedResults, bool completed, T leftSort, T rightSort)
        {
            SortedResults = sortedResults ?? throw new ArgumentNullException(nameof(sortedResults));
            Completed = completed;
            LeftSort = leftSort;
            RightSort = rightSort;
        }

        public SortResult(IReadOnlyList<T> sortedResults, T leftSort, T rightSort)
        {
            SortedResults = sortedResults ?? throw new ArgumentNullException(nameof(sortedResults));
            Completed = false;
            LeftSort = leftSort;
            RightSort = rightSort;
        }

        public SortResult(IReadOnlyList<T> sortedResults)
        {
            SortedResults = sortedResults ?? throw new ArgumentNullException(nameof(sortedResults));
            Completed = true;
            LeftSort = default(T);
            RightSort = default(T);
        }
    }
}
