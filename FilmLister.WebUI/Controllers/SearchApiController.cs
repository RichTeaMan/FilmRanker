using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FilmLister.Domain;
using FilmLister.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}