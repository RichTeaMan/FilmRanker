using FilmLister.Persistence;
using FilmLister.Service;
using FilmLister.Service.Exceptions;
using FilmLister.WebUI.Mappers;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Controllers
{
    [Route("api/filmListTemplate")]
    [ApiController]
    public class FilmListTemplateApiController : ControllerBase
    {
        private readonly FilmListerContext filmListerContext;

        private readonly FilmListTemplateMapper filmListTemplateMapper;

        private readonly FilmService filmService;

        public FilmListTemplateApiController(FilmListerContext filmListerContext, FilmListTemplateMapper filmListTemplateMapper, FilmService filmService)
        {
            this.filmListerContext = filmListerContext ?? throw new ArgumentNullException(nameof(filmListerContext));
            this.filmListTemplateMapper = filmListTemplateMapper ?? throw new ArgumentNullException(nameof(filmListTemplateMapper));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        [HttpPost("addFilm")]
        public async Task<ActionResult<Models.FilmListTemplate>> AddFilm([FromForm]int filmListTemplateId, [FromForm] int tmdbId)
        {
            var list = await filmService.RetrieveFilmListTemplateById(filmListTemplateId);
            var film = await filmService.RetrieveFilmByTmdbId(tmdbId);

            if (list != null && film != null)
            {
                try
                {
                    var updatedList = await filmService.AddFilmToFilmListTemplate(list, film);
                    var listModel = filmListTemplateMapper.Map(updatedList);
                    return listModel;
                }
                catch(FilmAlreadyInFilmListTemplateException)
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
