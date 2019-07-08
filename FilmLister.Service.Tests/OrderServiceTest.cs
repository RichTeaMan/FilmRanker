using FilmLister.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RichTea.Common.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.Service.Tests
{
    [TestClass]
    public class OrderServiceTest
    {
        private readonly Film testFilm = new Film(0, "test", 0, "test", "test", null, null, null, null, null);

        private OrderService orderService;

        [TestInitialize]
        public void TestInitialize()
        {
            orderService = new OrderService();
        }

        [TestMethod]
        public void SortCompleted()
        {
            var film1 = new OrderedFilm(1, false, testFilm);
            var film2 = new OrderedFilm(2, false, testFilm);

            film2.AddHigherRankedObject(film1);

            var sortResult = orderService.OrderFilms(new[] { film1, film2 });

            CollectionAssert.AreEqual(new[] { film2, film1 }, sortResult.SortedResults.ToArray());
            Assert.IsNull(sortResult.LeftSort);
            Assert.IsNull(sortResult.RightSort);
            Assert.IsTrue(sortResult.Completed);
        }

        [TestMethod]
        public void LongSortCompleted()
        {
            var film0 = new OrderedFilm(0, false, testFilm);
            var film1 = new OrderedFilm(1, false, testFilm);
            var film2 = new OrderedFilm(2, false, testFilm);
            var film3 = new OrderedFilm(3, false, testFilm);
            var film4 = new OrderedFilm(4, false, testFilm);
            var film5 = new OrderedFilm(5, false, testFilm);
            var film6 = new OrderedFilm(6, false, testFilm);
            var film7 = new OrderedFilm(7, false, testFilm);
            var film8 = new OrderedFilm(8, false, testFilm);
            var film9 = new OrderedFilm(9, false, testFilm);

            film0.AddHigherRankedObject(film1);
            film0.AddHigherRankedObject(film2);
            film0.AddHigherRankedObject(film3);
            film0.AddHigherRankedObject(film4);
            film0.AddHigherRankedObject(film5);
            film0.AddHigherRankedObject(film6);
            film0.AddHigherRankedObject(film7);
            film0.AddHigherRankedObject(film8);
            film0.AddHigherRankedObject(film9);

            film1.AddHigherRankedObject(film2);
            film1.AddHigherRankedObject(film3);
            film1.AddHigherRankedObject(film4);
            film1.AddHigherRankedObject(film5);
            film1.AddHigherRankedObject(film6);
            film1.AddHigherRankedObject(film7);
            film1.AddHigherRankedObject(film8);
            film1.AddHigherRankedObject(film9);

            film2.AddHigherRankedObject(film3);
            film2.AddHigherRankedObject(film4);
            film2.AddHigherRankedObject(film5);
            film2.AddHigherRankedObject(film6);
            film2.AddHigherRankedObject(film7);
            film2.AddHigherRankedObject(film8);
            film2.AddHigherRankedObject(film9);

            film3.AddHigherRankedObject(film4);
            film3.AddHigherRankedObject(film5);
            film3.AddHigherRankedObject(film6);
            film3.AddHigherRankedObject(film7);
            film3.AddHigherRankedObject(film8);
            film3.AddHigherRankedObject(film9);

            film4.AddHigherRankedObject(film5);
            film4.AddHigherRankedObject(film6);
            film4.AddHigherRankedObject(film7);
            film4.AddHigherRankedObject(film8);
            film4.AddHigherRankedObject(film9);

            film5.AddHigherRankedObject(film6);
            film5.AddHigherRankedObject(film7);
            film5.AddHigherRankedObject(film8);
            film5.AddHigherRankedObject(film9);

            film6.AddHigherRankedObject(film7);
            film6.AddHigherRankedObject(film8);
            film6.AddHigherRankedObject(film9);

            film7.AddHigherRankedObject(film8);
            film7.AddHigherRankedObject(film9);

            film8.AddHigherRankedObject(film9);

            var sortResult = orderService.OrderFilms(new[] {
                film3,
                film6,
                film0,
                film1,
                film9,
                film4,
                film2,
                film8,
                film5,
                film7 });

            CollectionAssert.AreEqual(new[] {
                film0,
                film1,
                film2,
                film3,
                film4,
                film5,
                film6,
                film7,
                film8,
                film9 },
            sortResult.SortedResults.ToArray());
            Assert.IsNull(sortResult.LeftSort);
            Assert.IsNull(sortResult.RightSort);
            Assert.IsTrue(sortResult.Completed);
        }

        [TestMethod]
        public void LongSortNotCompleted()
        {
            var film0 = new OrderedFilm(0, false, testFilm);
            var film1 = new OrderedFilm(1, false, testFilm);
            var film2 = new OrderedFilm(2, false, testFilm);
            var film3 = new OrderedFilm(3, false, testFilm);
            var film4 = new OrderedFilm(4, false, testFilm);
            var film5 = new OrderedFilm(5, false, testFilm);
            var film6 = new OrderedFilm(6, false, testFilm);
            var film7 = new OrderedFilm(7, false, testFilm);
            var film8 = new OrderedFilm(8, false, testFilm);
            var film9 = new OrderedFilm(9, false, testFilm);

            film0.AddHigherRankedObject(film4);
            film0.AddHigherRankedObject(film5);
            film0.AddHigherRankedObject(film6);
            film0.AddHigherRankedObject(film7);
            film0.AddHigherRankedObject(film8);
            film0.AddHigherRankedObject(film9);

            film1.AddHigherRankedObject(film2);
            film1.AddHigherRankedObject(film3);
            film1.AddHigherRankedObject(film8);
            film1.AddHigherRankedObject(film9);

            film2.AddHigherRankedObject(film3);
            film2.AddHigherRankedObject(film4);
            film2.AddHigherRankedObject(film5);
            film2.AddHigherRankedObject(film6);
            film2.AddHigherRankedObject(film7);
            film2.AddHigherRankedObject(film8);
            film2.AddHigherRankedObject(film9);

            film3.AddHigherRankedObject(film4);
            film3.AddHigherRankedObject(film5);
            film3.AddHigherRankedObject(film6);
            film3.AddHigherRankedObject(film8);
            film3.AddHigherRankedObject(film9);

            film6.AddHigherRankedObject(film7);
            film6.AddHigherRankedObject(film8);
            film6.AddHigherRankedObject(film9);

            film7.AddHigherRankedObject(film8);
            film7.AddHigherRankedObject(film9);

            film8.AddHigherRankedObject(film9);

            var sortResult = orderService.OrderFilms(new[] {
                film3,
                film6,
                film0,
                film1,
                film9,
                film4,
                film2,
                film8,
                film5,
                film7 });

            CollectionAssert.AllItemsAreUnique(sortResult.SortedResults.ToArray());
            Assert.AreEqual(10, sortResult.SortedResults.Count);
            Assert.IsNotNull(sortResult.LeftSort);
            Assert.IsNotNull(sortResult.RightSort);
            Assert.IsFalse(sortResult.Completed);
        }

        [TestMethod]
        public void SortNotCompleted()
        {
            var film1 = new OrderedFilm(1, false, testFilm);
            var film2 = new OrderedFilm(2, false, testFilm);

            var sortResult = orderService.OrderFilms(new[] { film1, film2 });

            Assert.IsNotNull(sortResult.LeftSort);
            Assert.IsNotNull(sortResult.RightSort);
            Assert.AreNotSame(sortResult.LeftSort, sortResult.RightSort);
            Assert.IsFalse(sortResult.Completed);
        }

        [TestMethod]
        public void SortLargeIntegerSet()
        {
            int setLength = 1000;

            var integerComparableDictionary = new Dictionary<int, IntegerAbstractComparable>();
            foreach (var i in Enumerable.Range(0, setLength))
            {
                integerComparableDictionary.Add(i, new IntegerAbstractComparable(i));
            }

            foreach (var kv in integerComparableDictionary)
            {
                if (integerComparableDictionary.TryGetValue(kv.Key + 1, out IntegerAbstractComparable greater))
                {
                    kv.Value.AddHigherRankedObject(greater);
                }
            }

            var integerComparableList = integerComparableDictionary.Values.RandomiseOrder().ToArray();
            var orderedIntegerComparableSortResult = orderService.OrderObjects(integerComparableList);

            var expectedIntegerComparableList = integerComparableList.OrderBy(i => i.Value).ToArray();

            // asserts
            Assert.IsTrue(orderedIntegerComparableSortResult.Completed);
            CollectionAssert.AreEquivalent(expectedIntegerComparableList, orderedIntegerComparableSortResult.SortedResults.ToArray());

            int comparisons = orderedIntegerComparableSortResult.SortedResults.Sum(i => i.Comparisons);
            Console.WriteLine(comparisons);
        }

        [TestMethod]
        public void CountSortDecisionsTest()
        {
            int setLength = 11;
            int sortAttempts = 1000;
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
                    orderedIntegerComparableSortResult = orderService.OrderObjects(integerComparableList);
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
            Console.WriteLine(averageDecision);
            // quick sort - 35.519
            // 13
            // 53

            // merge sort
            // min 17
            // max 52
        }
    }

    public class IntegerAbstractComparable : AbstractComparable<IntegerAbstractComparable>
    {
        public int Value { get; }

        public int Comparisons { get; private set; }

        public IntegerAbstractComparable(int value)
        {
            Value = value;
        }

        public override AbstractComparisonResult AbstractCompareTo(IntegerAbstractComparable other)
        {
            Comparisons++;
            return base.AbstractCompareTo(other);
        }

        public override IEqualityComparer<IntegerAbstractComparable> CreateComparer()
        {
            return new IntegerAbstractComparableComparer();
        }
    }

    public class IntegerAbstractComparableComparer : IEqualityComparer<IntegerAbstractComparable>
    {
        public bool Equals(IntegerAbstractComparable x, IntegerAbstractComparable y)
        {
            return x.Value == y.Value;
        }

        public int GetHashCode(IntegerAbstractComparable obj)
        {
            return obj.Value.GetHashCode();
        }
    }
}
