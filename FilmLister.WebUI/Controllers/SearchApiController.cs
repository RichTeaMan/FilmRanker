using FilmLister.Domain;
using FilmLister.Service;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchApiController : ControllerBase
    {
        private readonly FilmService filmService;

        public SearchApiController(FilmService filmService)
        {
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        [HttpGet("films/{query}")]
        public async Task<FilmTitle[]> Films(string query)
        {
            var titles = await filmService.SearchFilmTitles(query);
            return titles;
        }

        [HttpGet("filmsByPerson/{query}")]
        public async Task<FilmTitle[]> FilmsByPerson(string query)
        {
            var titles = await filmService.SearchFilmTitlesByPersonName(query);
            return titles;
        }
    }
}