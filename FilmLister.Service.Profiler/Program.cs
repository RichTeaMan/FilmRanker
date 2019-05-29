using FilmLister.Server.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FilmLister.Service.Profiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var orderService = new OrderService();
            int setLength = 2000;

            var integerComparableDictionary = new Dictionary<int, IntegerAbstractComparable>();
            foreach (var i in Enumerable.Range(0, setLength))
            {
                integerComparableDictionary.Add(i, new IntegerAbstractComparable(i));
            }

            foreach (var kv in integerComparableDictionary)
            {
                if (integerComparableDictionary.TryGetValue(kv.Key + 1, out IntegerAbstractComparable greater))
                {
                    kv.Value.HigherRankedObjects.Add(greater);
                }
            }

            var integerComparableList = integerComparableDictionary.Values.ToArray();
            var orderedIntegerComparableSortResult = orderService.OrderObjects(integerComparableList);

            var expectedIntegerComparableList = integerComparableList.OrderBy(i => i.Value).ToArray();

            // asserts
            Assert.IsTrue(orderedIntegerComparableSortResult.Completed);
            CollectionAssert.AreEquivalent(expectedIntegerComparableList, orderedIntegerComparableSortResult.SortedResults.ToArray());

            int comparisons = orderedIntegerComparableSortResult.SortedResults.Sum(i => i.Comparisons);
            System.Console.WriteLine(comparisons);
        }
    }
}
