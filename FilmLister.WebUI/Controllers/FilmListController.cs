using FilmLister.Domain;
using FilmLister.Service;
using FilmLister.Service.Exceptions;
using FilmLister.WebUI.Mappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Controllers
{
    public class FilmListController : Controller
    {
        private readonly FilmListMapper filmListMapper;

        private readonly FilmListTemplateMapper filmListTemplateMapper;

        private readonly FilmService filmService;

        public FilmListController(FilmListMapper filmListMapper, FilmListTemplateMapper filmListTemplateMapper, FilmService filmService)
        {
            this.filmListMapper = filmListMapper ?? throw new ArgumentNullException(nameof(filmListMapper));
            this.filmListTemplateMapper = filmListTemplateMapper ?? throw new ArgumentNullException(nameof(filmListTemplateMapper));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        public async Task<IActionResult> Index()
        {
            var lists = await filmService.RetrieveFilmListTemplates();
            var modelLists = lists.Select(l => filmListTemplateMapper.Map(l)).ToArray();

            return View(modelLists);
        }

        public async Task<IActionResult> Create(int listId)
        {
            int filmListId = await filmService.CreateOrderedFilmList(listId);
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