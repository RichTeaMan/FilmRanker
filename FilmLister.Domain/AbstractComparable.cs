using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmLister.Domain
{
    public abstract class AbstractComparable<T> : IComparable<T>, IComparable where T : AbstractComparable<T>
    {
        public HashSet<T> HigherRankedObjects { get; private set; }

        protected AbstractComparable()
        {
            HigherRankedObjects = new HashSet<T>(CreateComparer());
        }

        /// <summary>
        /// Allows a custom comparer to be provided. This may allow better performance.
        /// </summary>
        /// <returns></returns>
        public virtual IEqualityComparer<T> CreateComparer()
        {
            return EqualityComparer<T>.Default;
        }

        public virtual AbstractComparisonResult AbstractCompareTo(T other)
        {
            if (typeof(T) != GetType())
            {
                throw new InvalidOperationException($"Invalid type, comparison objects must be of the same type.");
            }

            AbstractComparisonResult result = new AbstractComparisonResult(0, false);

            if (other == this)
            {
                result = new AbstractComparisonResult(0, true);
            }
            else if (HigherRankedObjects.Contains(other))
            {
                result = new AbstractComparisonResult(-1, true);
            }
            else if (other.HigherRankedObjects.Contains((T)this))
            {
                result = new AbstractComparisonResult(1, true);
            }
            if (!result.ComparisonSucceeded)
            {
                var higherRankedObjects = FetchTransitiveHigherRankedObjects(result);
                if (higherRankedObjects.Any(o => o == other))
                {
                    result = new AbstractComparisonResult(-1, true);
                }
            }
            if (!result.ComparisonSucceeded)
            {
                var otherHigherRankedObjects = other.FetchTransitiveHigherRankedObjects(result);
                if (otherHigherRankedObjects.Any(o => o ==  this))
                {
                    result = new AbstractComparisonResult(1, true);
                }
            }
            return result;
        }

        /// <summary>
        /// Gets all objects that are ranked higher than this one by extrapolating transitive ranks.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public IEnumerable<T> FetchTransitiveHigherRankedObjects(AbstractComparisonResult result)
        {
            Stack<T> entitiesToFlatten = new Stack<T>(HigherRankedObjects);
            while (entitiesToFlatten.TryPop(out T entity))
            {
                yield return entity;
                foreach (var nextEntity in entity.HigherRankedObjects)
                {
                    entitiesToFlatten.Push(nextEntity);
                }
            }
        }

        /// <summary>
        /// Results of comparisons.
        /// </summary>
        /// <remarks>
        /// Less than zero: This instance precedes other in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as other.
        /// Greater than zero: This instance follows other in the sort order. 
        /// </remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(T other)
        {
            var result = AbstractCompareTo(other);
            if (result.ComparisonSucceeded)
            {
                return result.ComparisonResult;
            }
            else
            {
                throw new UnknownComparisonException<T>((T)this, other);
            }
        }

        public int CompareTo(object obj)
        {
            return CompareTo((T)obj);
        }
    }
}
