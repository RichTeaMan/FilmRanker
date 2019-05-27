using FilmLister.Domain;
using FilmLister.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace FilmLister.Server.Tests
{
    [TestClass]
    public class OrderServiceTest
    {
        private OrderService orderService;

        [TestInitialize]
        public void TestInitialize()
        {
            orderService = new OrderService();
        }

        [TestMethod]
        public void SortCompleted()
        {
            var film1 = new OrderedFilm(1, "1");
            var film2 = new OrderedFilm(2, "2");

            film2.HigherRankedObjects.Add(film1);

            var sortResult = orderService.OrderFilms(new[] { film1, film2 });

            CollectionAssert.AreEqual(new[] { film2, film1 }, sortResult.SortedResults.ToArray());
            Assert.IsNull(sortResult.LeftSort);
            Assert.IsNull(sortResult.RightSort);
            Assert.IsTrue(sortResult.Completed);
        }

        [TestMethod]
        public void LongSortCompleted()
        {
            var film0 = new OrderedFilm(0, "0");
            var film1 = new OrderedFilm(1, "1");
            var film2 = new OrderedFilm(2, "2");
            var film3 = new OrderedFilm(3, "3");
            var film4 = new OrderedFilm(4, "4");
            var film5 = new OrderedFilm(5, "5");
            var film6 = new OrderedFilm(6, "6");
            var film7 = new OrderedFilm(7, "7");
            var film8 = new OrderedFilm(8, "8");
            var film9 = new OrderedFilm(9, "9");

            film0.HigherRankedObjects.Add(film1);
            film0.HigherRankedObjects.Add(film2);
            film0.HigherRankedObjects.Add(film3);
            film0.HigherRankedObjects.Add(film4);
            film0.HigherRankedObjects.Add(film5);
            film0.HigherRankedObjects.Add(film6);
            film0.HigherRankedObjects.Add(film7);
            film0.HigherRankedObjects.Add(film8);
            film0.HigherRankedObjects.Add(film9);

            film1.HigherRankedObjects.Add(film2);
            film1.HigherRankedObjects.Add(film3);
            film1.HigherRankedObjects.Add(film4);
            film1.HigherRankedObjects.Add(film5);
            film1.HigherRankedObjects.Add(film6);
            film1.HigherRankedObjects.Add(film7);
            film1.HigherRankedObjects.Add(film8);
            film1.HigherRankedObjects.Add(film9);

            film2.HigherRankedObjects.Add(film3);
            film2.HigherRankedObjects.Add(film4);
            film2.HigherRankedObjects.Add(film5);
            film2.HigherRankedObjects.Add(film6);
            film2.HigherRankedObjects.Add(film7);
            film2.HigherRankedObjects.Add(film8);
            film2.HigherRankedObjects.Add(film9);

            film3.HigherRankedObjects.Add(film4);
            film3.HigherRankedObjects.Add(film5);
            film3.HigherRankedObjects.Add(film6);
            film3.HigherRankedObjects.Add(film7);
            film3.HigherRankedObjects.Add(film8);
            film3.HigherRankedObjects.Add(film9);

            film4.HigherRankedObjects.Add(film5);
            film4.HigherRankedObjects.Add(film6);
            film4.HigherRankedObjects.Add(film7);
            film4.HigherRankedObjects.Add(film8);
            film4.HigherRankedObjects.Add(film9);

            film5.HigherRankedObjects.Add(film6);
            film5.HigherRankedObjects.Add(film7);
            film5.HigherRankedObjects.Add(film8);
            film5.HigherRankedObjects.Add(film9);

            film6.HigherRankedObjects.Add(film7);
            film6.HigherRankedObjects.Add(film8);
            film6.HigherRankedObjects.Add(film9);

            film7.HigherRankedObjects.Add(film8);
            film7.HigherRankedObjects.Add(film9);

            film8.HigherRankedObjects.Add(film9);

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
        public void SortNotCompleted()
        {
            var film1 = new OrderedFilm(1, "1");
            var film2 = new OrderedFilm(2, "2");

            var sortResult = orderService.OrderFilms(new[] { film1, film2 });

            Assert.IsNotNull(sortResult.LeftSort);
            Assert.IsNotNull(sortResult.RightSort);
            Assert.AreNotSame(sortResult.LeftSort, sortResult.RightSort);
            Assert.IsFalse(sortResult.Completed);
        }
    }
}
