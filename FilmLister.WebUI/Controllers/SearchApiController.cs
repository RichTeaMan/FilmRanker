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
            string key = $"filmQuery/{query}";
            if (!memoryCache.TryGetValue(key, out FilmTitle[] filmTitles))
            {
                filmTitles = await filmService.SearchFilmTitles(query);
                memoryCache.Set(key, filmTitles);
            }
            return filmTitles;
        }

        [HttpGet("persons/{query}")]
        public async Task<Person[]> Persons(string query)
        {
            string key = $"personQuery/{query}";
            if (!memoryCache.TryGetValue(key, out Person[] personTitles))
            {
                personTitles = await filmService.SearchPersons(query);
                memoryCache.Set(key, personTitles);
            }
            return personTitles;
        }

        [HttpGet("filmsByPersonId/{id}")]
        public async Task<FilmTitleWithPersonCredit[]> FilmsByPersonId(int id)
        {
            string key = $"filmsByPersonId/{id}";
            if (!memoryCache.TryGetValue(key, out FilmTitleWithPersonCredit[] filmTitles))
            {
                filmTitles = await filmService.FetchFilmTitlesByPersonId(id);
                memoryCache.Set(key, filmTitles);
            }
            return filmTitles;
        }
    }
}