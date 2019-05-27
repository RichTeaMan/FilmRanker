using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilmLister.Domain.Test
{
    [TestClass]
    public class OrderedFilmComparisonTest
    {
        [TestMethod]
        [ExpectedException(typeof(UnknownComparisonException<OrderedFilm>))]
        public void NoComparison()
        {
            var film1 = new OrderedFilm(1, "1");
            var film2 = new OrderedFilm(2, "2");

            film1.CompareTo(film2);
        }

        [TestMethod]
        public void MinusOneComparison()
        {
            var film1 = new OrderedFilm(1, "1");
            var film2 = new OrderedFilm(2, "2");

            film1.HigherRankedObjects.Add(film2);
            var comparion = film1.CompareTo(film2);

            Assert.AreEqual(-1, comparion);
        }

        [TestMethod]
        public void PositiveOneComparison()
        {
            var film1 = new OrderedFilm(1, "1");
            var film2 = new OrderedFilm(2, "2");

            film2.HigherRankedObjects.Add(film1);
            var comparion = film2.CompareTo(film1);

            Assert.AreEqual(-1, comparion);
        }
    }
}
