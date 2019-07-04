using FilmLister.Persistence;
using FilmLister.Service;
using FilmLister.Service.Exceptions;
using FilmLister.WebUI.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Controllers
{
    public class FilmListController : Controller
    {
        private readonly ILogger logger;

        private readonly FilmListerContext filmListerContext;

        private readonly FilmListMapper filmListMapper;

        private readonly FilmService filmService;

        public FilmListController(
            ILogger<FilmListController> logger,
            FilmListerContext filmListerContext,
            FilmListMapper filmListMapper,
            FilmService filmService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.filmListerContext = filmListerContext ?? throw new ArgumentNullException(nameof(filmListerContext));
            this.filmListMapper = filmListMapper ?? throw new ArgumentNullException(nameof(filmListMapper));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var filmListTemplate = await filmService.CreateFilmList();
            return RedirectToAction("View", new { filmListId = filmListTemplate.Id });
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Clone(int filmListId)
        {
            var clonedFilmListTemplate = await filmService.CloneFilmList(filmListId);
            return RedirectToAction("View", new { filmListId = clonedFilmListTemplate.Id });
        }

        [HttpGet("/FilmList/{filmListId}")]
        public async Task<IActionResult> View(int filmListId)
        {
            IActionResult result;
            var list = await filmService.RetrieveFilmListById(filmListId);
            if (list != null)
            {
                var listModel = filmListMapper.Map(list);
                result = View(listModel);
            }
            else
            {
                result = NotFound("Film list template with given ID not found.");
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> AddFilm(int filmListId, int tmdbId)
        {
            IActionResult result;
            var list = await filmService.RetrieveFilmListById(filmListId);
            var film = await filmService.RetrieveFilmByTmdbId(tmdbId);

            if (list != null && film != null)
            {
                try
                {
                    var updatedList = await filmService.AddFilmToFilmList(list, film);
                    var listModel = filmListMapper.Map(updatedList);
                }
                catch (FilmListTemplatePublishedException)
                {
                    logger.LogInformation("Cannot add film to published film list template.");
                }
                result = RedirectToAction("View", new { filmListId = list.Id });
            }
            else
            {
                result = NotFound("Film list template with given ID not found.");
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFilm(int filmListId, int tmdbId)
        {
            IActionResult result;
            int id = filmListId;
            try
            {
                await filmService.RemoveFilmFromFilmList(filmListId, tmdbId);
                result = RedirectToAction("View", new { filmListId = id });
            }
            catch (FilmListTemplatePublishedException)
            {
                logger.LogInformation("Cannot remove from film published film list template.");
                result = RedirectToAction("View", new { filmListId = id });
            }
            catch (ListNotFoundException)
            {
                result = NotFound("Film list template with given ID not found.");
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Rename(Models.FilmList filmList)
        {
            if (!ModelState.IsValid)
            {
                return View(filmList);
            }
            try
            {
                await filmService.RenameFilmList(filmList.Id, filmList.Name);
            }
            catch (FilmListTemplatePublishedException)
            {
                logger.LogInformation("Cannot rename published film list template.");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is ListNotFoundException)
            {
                logger.LogError(ex, $"Exception renaming film list. ID: '{filmList.Id}'. New name: '{filmList.Name}'.");
            }

            return RedirectToAction("View", new { filmListId = filmList.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Publish(Models.FilmList filmList)
        {
            await filmService.PublishFilmList(filmList.Id);
            return RedirectToAction("View", new { filmListId = filmList.Id });
        }

        public async Task<IActionResult> List()
        {
            var lists = await filmService.RetrieveFilmLists();
            var modelTemplates = lists.Select(l => filmListMapper.Map(l)).ToArray();
            return View(modelTemplates);
        }

        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int filmListId)
        {
            IActionResult result;
            var list = await filmService.RetrieveFilmListById(filmListId);

            if (list != null)
            {
                var listModel = filmListMapper.Map(list);
                return View(listModel);
            }
            else
            {
                result = NotFound("Film list with given ID not found.");
            }
            return result;
        }

        [Authorize]
        public async Task<IActionResult> Delete(int filmListId)
        {
            await filmService.DeleteFilmListById(filmListId);
            return RedirectToAction(nameof(List));
        }
    }
}
