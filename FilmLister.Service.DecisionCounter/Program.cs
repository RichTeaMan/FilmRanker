using FilmLister.Service.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichTea.Common.Extensions;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.Service.DecisionCounter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            int setLength = 11;
            int sortAttempts = 1000;

            Console.WriteLine("FilmLister decision counter");
            Console.WriteLine("This program will test the average number of user");
            Console.WriteLine("decisions it will take to finish a list.");
            Console.WriteLine("This will take several minutes to execute if the debugger is attached.");

            Console.WriteLine($"Testing sets of {setLength} over {sortAttempts} iterations.");

            //TestSortAlgorithm(setLength, sortAttempts,
           //     new MergeSort<IntegerAbstractComparable>());
            TestSortAlgorithm(setLength, sortAttempts,
                new QuickSort<IntegerAbstractComparable>());

            Console.WriteLine("Completed. Press enter to close.");
            Console.Read();
        }

        private static void TestSortAlgorithm(
            int setLength,
        int sortAttempts,
        ISortAlgorithm<IntegerAbstractComparable> sortAlgorithm)
        {
            var orderService = new OrderService();

            var decisionCounts = new ConcurrentBag<int>();

            Parallel.ForEach(Enumerable.Range(0, sortAttempts), attempt =>
            {
                var integerComparableList = Enumerable.Range(0, setLength)
                .RandomiseOrder()
                .Select(i => new IntegerAbstractComparable(i))
                .ToArray();

                bool completed = false;
                int decisions = 0;
                SortResult<IntegerAbstractComparable> orderedIntegerComparableSortResult = null;
                while (!completed)
                {
                    orderedIntegerComparableSortResult = orderService
                        .OrderObjects(integerComparableList, sortAlgorithm);
                    completed = orderedIntegerComparableSortResult.Completed;
                    if (!completed)
                    {
                        if (orderedIntegerComparableSortResult.LeftSort.Value > orderedIntegerComparableSortResult.RightSort.Value)
                        {
                            orderedIntegerComparableSortResult.RightSort.AddHigherRankedObject(orderedIntegerComparableSortResult.LeftSort);
                        }
                        else
                        {
                            orderedIntegerComparableSortResult.LeftSort.AddHigherRankedObject(orderedIntegerComparableSortResult.RightSort);
                        }
                        decisions++;
                    }
                }

                var expectedIntegerComparableList = integerComparableList.OrderBy(i => i.Value).ToArray();

                // asserts
                Assert.IsTrue(orderedIntegerComparableSortResult.Completed);
                CollectionAssert.AreEquivalent(expectedIntegerComparableList, orderedIntegerComparableSortResult.SortedResults.ToArray());

                decisionCounts.Add(decisions);
            });

            var averageDecision = decisionCounts.Average();
            var minDecision = decisionCounts.Min();
            var maxDecision = decisionCounts.Max();
            var diff = maxDecision - minDecision;

            Console.WriteLine($"{sortAlgorithm.GetType().Name} average decisions: {averageDecision} minimum decisions: {minDecision} maximum decisions: {maxDecision}");
        }
    }
}
