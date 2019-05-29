using System;

namespace FilmLister.Domain
{
    public struct AbstractComparisonResult
    {
        public int ComparisonResult { get; }

        public bool ComparisonSucceeded { get; }

        public AbstractComparisonResult(int comparisonResult, bool comparisonSucceeded)
        {
            ComparisonResult = comparisonResult;
            ComparisonSucceeded = comparisonSucceeded;
        }
    }
}
