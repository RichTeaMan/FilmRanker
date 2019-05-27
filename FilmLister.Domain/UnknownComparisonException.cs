using System;

namespace FilmLister.Domain
{
    public class UnknownComparisonException<T> : Exception where T : AbstractComparable<T>
    {
        public T Left { get; }

        public T Right { get; }

        public UnknownComparisonException(T left, T right)
        {
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }
    }
}
