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
    public class FilmListTemplateController : Controller
    {
        private readonly ILogger logger;

        private readonly FilmListerContext filmListerContext;

        private readonly FilmListTemplateMapper filmListTemplateMapper;

        private readonly FilmService filmService;

        public FilmListTemplateController(
            ILogger<FilmListTemplateController> logger,
            FilmListerContext filmListerContext,
            FilmListTemplateMapper filmListTemplateMapper,
            FilmService filmService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.filmListerContext = filmListerContext ?? throw new ArgumentNullException(nameof(filmListerContext));
            this.filmListTemplateMapper = filmListTemplateMapper ?? throw new ArgumentNullException(nameof(filmListTemplateMapper));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var filmListTemplate = await filmService.CreateFilmListTemplate();
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
                try
                {
                    var updatedList = await filmService.AddFilmToFilmListTemplate(list, film);
                    var listModel = filmListTemplateMapper.Map(updatedList);
                }
                catch (FilmListTemplatePublishedException)
                {
                    logger.LogInformation("Cannot add film to published film list template.");
                }
                result = RedirectToAction("View", new { filmListTemplateId = list.Id });
            }
            else
            {
                result = NotFound("Film list template with given ID not found.");
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFilm(int filmListTemplateId, int tmdbId)
        {
            IActionResult result;
            int id = filmListTemplateId;
            try
            {
                await filmService.RemoveFilmFromFilmListTemplate(filmListTemplateId, tmdbId);
                result = RedirectToAction("View", new { filmListTemplateId = id });
            }
            catch (FilmListTemplatePublishedException)
            {
                logger.LogInformation("Cannot remove from film published film list template.");
                result = RedirectToAction("View", new { filmListTemplateId = id });
            }
            catch (ListNotFoundException)
            {
                result = NotFound("Film list template with given ID not found.");
            }
            return result;
        }

        [HttpPost]
        public async Task<IActionResult> Rename(Models.FilmListTemplate filmListTemplate)
        {
            if (!ModelState.IsValid)
            {
                return View(filmListTemplate);
            }
            try
            {
                await filmService.RenameFilmListTemplate(filmListTemplate.Id, filmListTemplate.Name);
            }
            catch (FilmListTemplatePublishedException)
            {
                logger.LogInformation("Cannot rename published film list template.");
            }
            catch (Exception ex) when (ex is ArgumentException || ex is ListNotFoundException)
            {
                logger.LogError(ex, $"Exception renaming film list template. ID: '{filmListTemplate.Id}'. New name: '{filmListTemplate.Name}'.");
            }

            return RedirectToAction("View", new { filmListTemplateId = filmListTemplate.Id });
        }

        [HttpPost]
        public async Task<IActionResult> Publish(Models.FilmListTemplate filmListTemplate)
        {
            await filmService.PublishFilmListTemplate(filmListTemplate.Id);
            return RedirectToAction("View", new { filmListTemplateId = filmListTemplate.Id });
        }

        public async Task<IActionResult> List()
        {
            var templates = await filmService.RetrieveFilmListTemplates();
            var modelTemplates = templates.Select(l => filmListTemplateMapper.Map(l)).ToArray();
            return View(modelTemplates);
        }

        [Authorize]
        public async Task<IActionResult> ConfirmDelete(int filmListTemplateId)
        {
            IActionResult result;
            var list = await filmService.RetrieveFilmListTemplateById(filmListTemplateId);

            if (list != null)
            {
                var listModel = filmListTemplateMapper.Map(list);
                return View(listModel);
            }
            else
            {
                result = NotFound("Film list template with given ID not found.");
            }
            return result;
        }

        [Authorize]
        public async Task<IActionResult> Delete(int filmListTemplateId)
        {
            await filmService.DeleteFilmListTemplateById(filmListTemplateId);
            return RedirectToAction(nameof(List));
        }
    }
}
