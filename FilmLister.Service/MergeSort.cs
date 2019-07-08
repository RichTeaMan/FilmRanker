namespace FilmLister.Service {
  using System;

    public class MergeSort<T> : ISortAlgorithm<T> where T : IComparable
    {
        #region Constants
        private const int mergesDefault = 6;
        private const int insertionLimitDefault = 12;
        #endregion

        #region Properties
        protected int[] Positions { get; set; }

        private int merges;
        public int Merges
        {
            get { return merges; }
            set
            {
                // A minimum of 2 merges are required
                if (value > 1)
                    merges = value;
                else
                    throw new ArgumentOutOfRangeException();

                if (Positions == null || Positions.Length != merges)
                    Positions = new int[merges];
            }
        }

        public int InsertionLimit { get; set; }
        #endregion

        #region Constructors
        public MergeSort(int merges, int insertionLimit)
        {
            Merges = merges;
            InsertionLimit = insertionLimit;
        }

        public MergeSort()
          : this(mergesDefault, insertionLimitDefault)
        {
        }
        #endregion

        #region Sort Methods
        public void Sort(T[] entries)
        {
            // Allocate merge buffer
            var entries2 = new T[entries.Length];
            Sort(entries, entries2, 0, entries.Length - 1);
        }

        // Top-Down K-way Merge Sort
        public void Sort(T[] entries1, T[] entries2, int first, int last)
        {
            var length = last + 1 - first;
            if (length < 2)
                return;
            else if (length < InsertionLimit)
            {
                InsertionSort<T>.Sort(entries1, first, last);
                return;
            }

            var left = first;
            var size = Ceiling(length, Merges);
            for (var remaining = length; remaining > 0; remaining -= size, left += size)
            {
                var right = left + Math.Min(remaining, size) - 1;
                Sort(entries1, entries2, left, right);
            }

            Merge(entries1, entries2, first, last);
            Array.Copy(entries2, first, entries1, first, length);
        }
        #endregion

        #region Merge Methods
        public void Merge(T[] entries1, T[] entries2, int first, int last)
        {
            Array.Clear(Positions, 0, Merges);
            // This implementation has a quadratic time dependency on the number of merges
            for (var index = first; index <= last; index++)
                entries2[index] = remove(entries1, first, last);
        }

        private T remove(T[] entries, int first, int last)
        {
            var entry = default(T);
            var found = (int?)null;
            var length = last + 1 - first;

            var index = 0;
            var left = first;
            var size = Ceiling(length, Merges);
            for (var remaining = length; remaining > 0; remaining -= size, left += size, index++)
            {
                var position = Positions[index];
                if (position < Math.Min(remaining, size))
                {
                    var next = entries[left + position];
                    if (!found.HasValue || entry.CompareTo(next) > 0)
                    {
                        found = index;
                        entry = next;
                    }
                }
            }

            // Remove entry
            Positions[found.Value]++;
            return entry;
        }
        #endregion

        #region Math Methods
        private static int Ceiling(int numerator, int denominator)
        {
            return (numerator + denominator - 1) / denominator;
        }
        #endregion
        #region Insertion Sort
        static class InsertionSort
        {
            public static void Sort(T[] entries, int first, int last)
            {
                for (var index = first + 1; index <= last; index++)
                    Insert(entries, first, index);
            }

            private static void Insert(T[] entries, int first, int index)
            {
                var entry = entries[index];
                while (index > first && entries[index - 1].CompareTo(entry) > 0)
                    entries[index] = entries[--index];
                entries[index] = entry;
            }
        }
        #endregion
    }
}
