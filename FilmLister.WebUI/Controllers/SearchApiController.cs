using FilmLister.Domain;
using FilmLister.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace FilmLister.WebUI.Controllers
{
    [Route("api/search")]
    [ApiController]
    public class SearchApiController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly FilmService filmService;

        public SearchApiController(IMemoryCache memoryCache, FilmService filmService)
        {
            this.memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            this.filmService = filmService ?? throw new ArgumentNullException(nameof(filmService));
        }

        [HttpGet("films/{query}")]
        public async Task<FilmTitle[]> Films(string query)
        {
            FilmTitle[] filmTitles;
            string key = $"filmQuery/{query}";
            if (!memoryCache.TryGetValue(key, out filmTitles))
            {
                filmTitles = await filmService.SearchFilmTitles(query);
                memoryCache.Set(key, filmTitles);
            }
            return filmTitles;
        }

        [HttpGet("filmsByPerson/{query}")]
        public async Task<FilmTitleWithPersonCredit[]> FilmsByPerson(string query)
        {
            FilmTitleWithPersonCredit[] filmTitles;
            string key = $"personQuery/{query}";
            if (!memoryCache.TryGetValue(key, out filmTitles))
            {
                filmTitles = await filmService.SearchFilmTitlesByPersonName(query);
                memoryCache.Set(key, filmTitles);
            }
            return filmTitles;
        }
    }
}