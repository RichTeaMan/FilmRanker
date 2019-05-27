using FilmLister.Persistence;
using FilmLister.TmdbIntegration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.Service
{
    public class FilmService
    {
        private readonly TmdbService tmdbService;

        private readonly FilmListerContext filmListerContext;

        public FilmService(TmdbService tmdbService, FilmListerContext filmListerContext)
        {
            this.tmdbService = tmdbService ?? throw new ArgumentNullException(nameof(tmdbService));
            this.filmListerContext = filmListerContext ?? throw new ArgumentNullException(nameof(filmListerContext));
        }

        public async Task<Domain.Film[]> RetrieveFilms()
        {
            var films = await filmListerContext.Films.ToArrayAsync();

            var domainFilms = films.Select(f => Map(f)).ToArray();
            return domainFilms;
        }

        public async Task<Domain.Film> RetrieveFilmByTmdbId(int tmdbId)
        {
            var film = await filmListerContext.Films.FirstOrDefaultAsync(f => f.TmdbId == tmdbId);
            if (film == null)
            {
                // try tmdb
                var movieDetail = await tmdbService.FetchMovieDetails(tmdbId);
                film = new Persistence.Film
                {
                    Name = movieDetail.title,
                    ImageUrl = $"https://image.tmdb.org/t/p/w500/{movieDetail.poster_path}",
                    ImdbId = movieDetail.imdb_id,
                    TmdbId = tmdbId
                };

                await filmListerContext.Films.AddAsync(film);
                await filmListerContext.SaveChangesAsync();
            }

            var domainFilm = Map(film);
            return domainFilm;
        }

        public async Task<Domain.FilmListTemplate> RetrieveFilmListTemplateById(int id)
        {
            var list = await filmListerContext.FilmListTemplates
                .Include(l => l.FilmListItems)
                    .ThenInclude(i => i.Film)
                .FirstOrDefaultAsync(l => l.Id == id);
            var domainList = Map(list);
            return domainList;
        }

        public async Task<Domain.FilmListTemplate> AddFilmToFilmListTemplate(Domain.FilmListTemplate filmListTemplate, Domain.Film film)
        {
            var persistenceList = await filmListerContext.FilmListTemplates.FirstAsync(l => l.Id == filmListTemplate.Id);
            var persistenceFilm = await filmListerContext.Films.FirstAsync(l => l.Id == film.Id);

            if (persistenceList.FilmListItems == null)
            {
                persistenceList.FilmListItems = new List<FilmListItem>();
            }

            persistenceList.FilmListItems.Add(new FilmListItem
            {
                Film = persistenceFilm,
                FilmListTemplate = persistenceList
            });
            await filmListerContext.SaveChangesAsync();

            var list = await RetrieveFilmListTemplateById(persistenceList.Id);
            return list;
        }

        private Domain.Film Map(Persistence.Film film)
        {
            Domain.Film filmModel = null;
            if (film != null)
            {
                filmModel = new Domain.Film(film.Id, film.Name, film.TmdbId, film.ImageUrl, film.ImdbId);
            }
            return filmModel;
        }

        private Domain.FilmListTemplate Map(Persistence.FilmListTemplate orderedFilmList)
        {
            List<Domain.Film> films = new List<Domain.Film>();
            if (orderedFilmList?.FilmListItems != null)
            {
                films.AddRange(orderedFilmList.FilmListItems.Select(f => Map(f.Film)));
            }
            var filmList = new Domain.FilmListTemplate(
                orderedFilmList.Id,
                orderedFilmList.Name,
                films.ToArray());
            return filmList;
        }
    }
}
