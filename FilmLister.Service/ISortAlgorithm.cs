using System;
using System.Collections.Generic;
using System.Text;

namespace FilmLister.Service
{
    public interface ISortAlgorithm<T> where T : IComparable
    {
        void Sort(T[] entries);
    }
}
