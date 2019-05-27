using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmLister.Domain;
using FilmLister.Persistence;
using FilmLister.Service;
using FilmLister.WebUI.Mappers;
using FilmLister.WebUI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmLister.WebUI.Controllers
{
    public class FilmListTemplateController : Controller
    {
        private readonly FilmListerContext filmListerContext;

        private readonly FilmListTemplateMapper filmListTemplateMapper;

        private readonly FilmService filmService;

        public FilmListTemplateController(FilmListerContext filmListerContext, FilmListTemplateMapper filmListTemplateMapper, FilmService filmService)
        {
            this.filmListerContext = filmListerContext ?? throw new ArgumentNullException(nameof(filmListerContext));
            this.filmListTemplateMapper = filmListTemplateMapper ?? throw new ArgumentNullException(nameof(filmListTemplateMapper));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string listName)
        {
            var filmListTemplate = new Persistence.FilmListTemplate()
            {
                Name = listName
            };
            await filmListerContext.FilmListTemplates.AddAsync(filmListTemplate);
            await filmListerContext.SaveChangesAsync();

            return RedirectToAction("View", new { filmListTemplateId = filmListTemplate.Id });
        }

        public async Task<IActionResult> View(int filmListTemplateId)
        {
            IActionResult result;
            var list = await filmService.RetrieveFilmListTemplateById(filmListTemplateId);
            if (list != null)
            {
                var listModel = filmListTemplateMapper.Map(list);
                result = View(listModel);
            }
            else
            {
                result = NotFound("Film list template with given ID not found.");
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> AddFilm(int filmListTemplateId, int tmdbId)
        {
            IActionResult result;
            var list = await filmService.RetrieveFilmListTemplateById(filmListTemplateId);
            var film = await filmService.RetrieveFilmByTmdbId(tmdbId);

            if (list != null && film != null)
            {
                var updatedList = await filmService.AddFilmToFilmListTemplate(list, film);
                var listModel = filmListTemplateMapper.Map(updatedList);
                result = RedirectToAction("View", new { filmListTemplateId = list.Id });
            }
            else
            {
                result = NotFound("Film list template with given ID not found.");
            }
            return result;
        }
    }
}
