using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FilmLister.Domain.Test
{
    [TestClass]
    public class OrderedFilmComparisonTest
    {
        private Film  testFilm = new Film(0, "test", 0, "test", "test", null);

        [TestMethod]
        [ExpectedException(typeof(UnknownComparisonException<OrderedFilm>))]
        public void NoComparison()
        {
            var film1 = new OrderedFilm(1, testFilm);
            var film2 = new OrderedFilm(2, testFilm);

            film1.CompareTo(film2);
        }

        [TestMethod]
        public void MinusOneComparison()
        {
            var film1 = new OrderedFilm(1, testFilm);
            var film2 = new OrderedFilm(2, testFilm);

            film1.AddHigherRankedObject(film2);
            var comparion = film1.CompareTo(film2);

            Assert.AreEqual(-1, comparion);
        }

        [TestMethod]
        public void PositiveOneComparison()
        {
            var film1 = new OrderedFilm(1, testFilm);
            var film2 = new OrderedFilm(2, testFilm);

            film2.AddHigherRankedObject(film1);
            var comparion = film2.CompareTo(film1);

            Assert.AreEqual(-1, comparion);
        }
    }
}
