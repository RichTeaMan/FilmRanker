using FilmLister.Service;
using FilmLister.Service.Exceptions;
using FilmLister.WebUI.Mappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Controllers
{
    public class FilmRankController : Controller
    {
        private readonly FilmRankMapper filmRankMapper;

        private readonly FilmListMapper filmListMapper;

        private readonly FilmService filmService;

        public FilmRankController(FilmRankMapper filmRankMapper, FilmListMapper filmListMapper, FilmService filmService)
        {
            this.filmRankMapper = filmRankMapper ?? throw new ArgumentNullException(nameof(filmRankMapper));
            this.filmListMapper = filmListMapper ?? throw new ArgumentNullException(nameof(filmListMapper));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        public async Task<IActionResult> Index()
        {
            var lists = await filmService.RetrieveFilmLists();
            var modelLists = lists.Select(l => filmListMapper.Map(l)).ToArray();

            return View(modelLists);
        }

        [HttpGet("/FilmRank/{id}")]
        public async Task<IActionResult> Index(int id)
        {
            IActionResult result;
            try
            {
                var orderedFilmList = await filmService.AttemptRankOrder(id);
                var filmList = filmRankMapper.Map(orderedFilmList);
                if (filmList.Completed)
                {
                    result = View("CompletedFilmRank", filmList);
                }
                else
                {
                    result = View("FilmRank", filmList);
                }
            }
            catch (ListNotFoundException)
            {
                result = NotFound();
            }
            return result;
        }

        public async Task<IActionResult> Create(int listId)
        {
            int filmListId = await filmService.CreateFilmRankFromFilmList(listId);
            return RedirectToAction("Index", new { id = filmListId });
        }

        public async Task<IActionResult> SubmitFilmChoice(int id, int lesserFilmId, int greaterFilmId)
        {
            IActionResult result;
            try
            {
                await filmService.SubmitFilmChoice(id, lesserFilmId, greaterFilmId);
                int _id = id;
                result = RedirectToAction("Index", new { id = _id });
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

        public async Task<IActionResult> SubmitIgnoreFilm(int filmRankId, int filmId)
        {
            await filmService.SubmitIgnoreFilm(filmRankId, filmId);
            return RedirectToAction("Index", new { id = filmRankId });
        }
    }
}