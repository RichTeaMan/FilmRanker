using FilmLister.Persistence;
using FilmLister.Service;
using FilmLister.Service.Exceptions;
using FilmLister.WebUI.Mappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Controllers
{
    [Route("api/filmList")]
    [ApiController]
    public class FilmListApiController : ControllerBase
    {
        private readonly ILogger logger;

        private readonly FilmListerContext filmListerContext;

        private readonly FilmListMapper filmListMapper;

        private readonly FilmService filmService;

        public FilmListApiController(
            ILogger<FilmListApiController> logger,
            FilmListerContext filmListerContext,
            FilmListMapper filmListTemplateMapper,
            FilmService filmService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.filmListerContext = filmListerContext ?? throw new ArgumentNullException(nameof(filmListerContext));
            this.filmListMapper = filmListTemplateMapper ?? throw new ArgumentNullException(nameof(filmListTemplateMapper));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        [HttpPost("addFilm")]
        public async Task<ActionResult<Models.FilmList>> AddFilm([FromForm]int filmListId, [FromForm] int tmdbId)
        {
            var list = await filmService.RetrieveFilmListById(filmListId);
            var film = await filmService.RetrieveFilmByTmdbId(tmdbId);

            if (list != null && film != null)
            {
                try
                {
                    var updatedList = await filmService.AddFilmToFilmList(list, film);
                    var listModel = filmListMapper.Map(updatedList);
                    return listModel;
                }
                catch (FilmListTemplatePublishedException)
                {
                    logger.LogInformation("Cannot add film to published film list.");
                    var listModel = filmListMapper.Map(list);
                    return listModel;
                }
                catch (FilmAlreadyInFilmListTemplateException)
                {
                    return BadRequest($"Lists cannot have duplicate films. {film.Name} is already in the list.");
                }
            }
            else
            {
                return NotFound();
            }
        }
    }
}
