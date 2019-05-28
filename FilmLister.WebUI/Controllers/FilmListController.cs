using FilmLister.Domain;
using FilmLister.Service;
using FilmLister.Service.Exceptions;
using FilmLister.WebUI.Mappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Controllers
{
    public class FilmListController : Controller
    {
        private readonly OrderService orderService;

        private readonly FilmListMapper filmListMapper;

        private readonly FilmService filmService;

        private static readonly Dictionary<string, OrderedFilmList> FilmList = new Dictionary<string, OrderedFilmList>();

        public FilmListController(OrderService orderService, FilmListMapper filmListMapper, FilmService filmService)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.filmListMapper = filmListMapper ?? throw new ArgumentNullException(nameof(filmListMapper));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Create()
        {
            int filmListId = await filmService.CreateOrderedFilmList(1);
            return RedirectToAction("List", new { id = filmListId });
        }

        public async Task<IActionResult> SubmitFilmChoice(int id, int lesserFilmId, int greaterFilmId)
        {
            IActionResult result;
            try
            {
                await filmService.SubmitFilmChoice(id, lesserFilmId, greaterFilmId);
                result = RedirectToAction("List", new { id = id });
            }
            catch (FilmNotFoundException)
            {
                result = NotFound("Film with given ID not found.");
            }
            catch (ListNotFoundException)
            {
                result = NotFound("Film list with given ID not found.");

            }
            return result;
        }

        public async Task<IActionResult> List(int id)
        {
            IActionResult result;
            try
            {
                var orderedFilmList = await filmService.AttemptListOrder(id);
                var filmList = filmListMapper.Map(orderedFilmList);
                result = View("FilmList", filmList);
            }
            catch (ListNotFoundException)
            {
                result = NotFound();
            }
            return result;
        }
    }
}