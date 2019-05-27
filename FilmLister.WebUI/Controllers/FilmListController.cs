using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmLister.Domain;
using FilmLister.Service;
using FilmLister.WebUI.Mappers;
using FilmLister.WebUI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmLister.WebUI.Controllers
{
    public class FilmListController : Controller
    {
        private readonly OrderService orderService;

        private readonly FilmListMapper filmListMapper;

        private readonly static Film[] Films = new[] { new Film() {Id = 1, Name = "Jurassic Park" }, new Film() { Id = 2, Name = "Jurassic Park 2: The Lost World" }, new Film() { Id = 3, Name = "Jurassic Park 3" } };

        private readonly static Dictionary<string, OrderedFilmList> FilmList = new Dictionary<string, OrderedFilmList>();

        public FilmListController(OrderService orderService, FilmListMapper filmListMapper)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.filmListMapper = filmListMapper ?? throw new ArgumentNullException(nameof(filmListMapper));
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var id = Guid.NewGuid().ToString();
            var filmList = new OrderedFilmList()
            {
                Id = id,
                Completed = false,
                SortedFilms = Films.Select(f => new OrderedFilm(f.Id, f.Name)).ToArray()
            };
            FilmList.Add(id, filmList);
            return RedirectToAction("List", new { id = id });
        }

        public IActionResult SubmitFilmChoice(string id, int lesserFilmId, int greaterFilmId)
        {
            IActionResult result;
            if (FilmList.TryGetValue(id, out OrderedFilmList orderedFilmList))
            {
                var lesser = orderedFilmList.SortedFilms.FirstOrDefault(f => f.Id == lesserFilmId);
                var greater = orderedFilmList.SortedFilms.FirstOrDefault(f => f.Id == greaterFilmId);

                if (lesser == null || greater == null)
                {
                    result = NotFound("Film with given ID not found.");
                }
                else
                {
                    lesser.HigherRankedObjects.Add(greater);
                    result = RedirectToAction("List", new { id = id });
                }
            }
            else
            {
                result = NotFound("Film list with given ID not found.");
            }
            return result;
        }

        public IActionResult List(string id)
        {
            IActionResult result;
            if (FilmList.TryGetValue(id, out OrderedFilmList orderedFilmList))
            {
                var orderResult = orderService.OrderFilms(orderedFilmList.SortedFilms);

                orderedFilmList.SortedFilms = orderResult.SortedResults.ToArray();
                orderedFilmList.ChoiceA = orderResult.LeftSort;
                orderedFilmList.ChoiceB = orderResult.RightSort;
                orderedFilmList.Completed = orderResult.Completed;

                var filmList = filmListMapper.Map(orderedFilmList);
                result = View("FilmList", filmList);
            }
            else
            {
                result = NotFound();
            }
            return result;
        }
    }
}