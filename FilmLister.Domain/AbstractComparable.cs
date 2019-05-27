using System;
using System.Collections.Generic;
using System.Text;

namespace FilmLister.Domain
{
    public abstract class AbstractComparable<T> : IComparable<T>, IComparable where T : AbstractComparable<T>
    {
        public HashSet<T> HigherRankedObjects { get; } = new HashSet<T>();

        /// <summary>
        /// Results of comparions.
        /// </summary>
        /// <remarks>
        /// Less than zero: This instance precedes other in the sort order.
        /// Zero: This instance occurs in the same position in the sort order as other. This method will never return 0.
        /// Greater than zero: This instance follows other in the sort order. 
        /// </remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(T other)
        {
            if (typeof(T) != GetType()) {
                throw new InvalidOperationException($"Invalid type, comparison objects must be of the same type.");
            }

            if (HigherRankedObjects.Contains(other))
            {
                return -1;
            }
            if (other.HigherRankedObjects.Contains((T)this))
            {
                return 1;
            }
            throw new UnknownComparisonException<T>((T)this, other);
        }

        public int CompareTo(object obj)
        {
            return CompareTo((T)obj);
        }
    }
}
