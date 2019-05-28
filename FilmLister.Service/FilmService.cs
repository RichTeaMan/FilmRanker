using FilmLister.Domain;
using FilmLister.Persistence;
using FilmLister.Service.Exceptions;
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
        private readonly OrderService orderService;

        private readonly TmdbService tmdbService;

        private readonly FilmListerContext filmListerContext;

        public FilmService(OrderService orderService, TmdbService tmdbService, FilmListerContext filmListerContext)
        {
            this.orderService = orderService ?? throw new ArgumentNullException(nameof(orderService));
            this.tmdbService = tmdbService ?? throw new ArgumentNullException(nameof(tmdbService));
            this.filmListerContext = filmListerContext ?? throw new ArgumentNullException(nameof(filmListerContext));
        }

        public async Task<Domain.Film[]> RetrieveFilms()
        {
            var films = await filmListerContext.Films.ToArrayAsync();

            var domainFilms = films.Select(f => Map(f)).ToArray();
            return domainFilms;
        }

        public async Task<Domain.FilmListTemplate[]> RetrieveFilmListTemplates()
        {
            var filmListTemplates = await filmListerContext.FilmListTemplates.ToArrayAsync();

            var domainLists = filmListTemplates.Select(l => Map(l)).ToArray();
            return domainLists;
        }

        /// <summary>
        /// Creates a new list for ordering. Returns ID of the list.
        /// </summary>
        /// <param name="filmListTemplateId"></param>
        /// <returns></returns>
        public async Task<int> CreateOrderedFilmList(int filmListTemplateId)
        {
            var filmListTemplate = await RetrieveFilmListTemplateById(filmListTemplateId);
            var persistenceListTemplate = await filmListerContext.FilmListTemplates
                .Include(l => l.FilmListItems)
                    .ThenInclude(i => i.Film)
                .FirstAsync(l => l.Id == filmListTemplateId);

            var films = persistenceListTemplate.FilmListItems.Select(i => new Persistence.OrderedFilm() { Film = i.Film }).ToList();
            var filmList = new Persistence.OrderedList()
            {
                OrderedFilms = films,
                FilmListTemplate = persistenceListTemplate
            };

            await filmListerContext.OrderedLists.AddAsync(filmList);
            await filmListerContext.SaveChangesAsync();

            return filmList.Id;
        }

        public async Task SubmitFilmChoice(int id, int lesserFilmId, int greaterFilmId)
        {
            var orderedFilmList = await filmListerContext.OrderedLists
                .Include(ol => ol.OrderedFilms)
                    .ThenInclude(f => f.Film)
                .FirstOrDefaultAsync(ol => ol.Id == id);
            if (orderedFilmList != null)
            {
                var lesser = orderedFilmList.OrderedFilms.FirstOrDefault(f => f.Id == lesserFilmId);
                var greater = orderedFilmList.OrderedFilms.FirstOrDefault(f => f.Id == greaterFilmId);

                if (lesser == null)
                {
                    throw new FilmNotFoundException(lesserFilmId);
                }
                else if (greater == null)
                {
                    throw new FilmNotFoundException(greaterFilmId);
                }
                else
                {
                    if (lesser.GreaterRankedFilmItems == null)
                    {
                        lesser.GreaterRankedFilmItems = new List<Persistence.OrderedFilmRankItem>();
                    }
                    var rankItem = new OrderedFilmRankItem
                    {
                        LesserRankedFilm = lesser,
                        GreaterRankedFilm = greater
                    };
                    await filmListerContext.OrderedFilmRankItems.AddAsync(rankItem);
                    await filmListerContext.SaveChangesAsync();
                    System.Console.WriteLine();
                }
            }
            else
            {
                throw new ListNotFoundException(id);
            }
        }

        /// <summary>
        /// Attempts to order a list. Further information might be required, which is requested in the return object.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<OrderedFilmList> AttemptListOrder(int id)
        {
            var persistenceOrderedFilmList = await filmListerContext.OrderedLists
                .Include(ol => ol.OrderedFilms)
                    .ThenInclude(f => f.GreaterRankedFilmItems)
                .Include(ol => ol.OrderedFilms)
                    .ThenInclude(f => f.Film)
                .FirstOrDefaultAsync(ol => ol.Id == id);

            if (persistenceOrderedFilmList == null)
            {
                throw new ListNotFoundException(id);
            }

            var orderedFilmList = Map(persistenceOrderedFilmList);
            var orderResult = orderService.OrderFilms(orderedFilmList.SortedFilms);

            var filmList = new Domain.OrderedFilmList(
                orderedFilmList.Id,
                orderResult.Completed,
                orderResult.SortedResults.ToArray(),
                orderResult.LeftSort,
                orderResult.RightSort);

            if (persistenceOrderedFilmList.Completed != filmList.Completed)
            {
                persistenceOrderedFilmList.Completed = filmList.Completed;
                await filmListerContext.SaveChangesAsync();
            }

            return filmList;
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
                    ImageUrl = CreateFullImagePath(movieDetail.poster_path),
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

        /// <summary>
        /// Creates a full image path from part of a path. If image path is null
        /// then an empty string is returned.
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public string CreateFullImagePath(string imagePath)
        {
            string fullPath = string.Empty;
            if (!string.IsNullOrWhiteSpace(imagePath))
            {
                fullPath = $"https://image.tmdb.org/t/p/w500/{imagePath}";
            }
            return fullPath;
        }

        public async Task<FilmTitle[]> SearchFilmTitles(string query)
        {
            var searchResult = await tmdbService.SearchMovies(query);
            var titles = searchResult.results
                .Select(r => new FilmTitle(r.id, r.title, CreateFullImagePath(r.poster_path), r.release_date))
                .ToArray();
            return titles;
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

        private Domain.OrderedFilm Map(Persistence.OrderedFilm film)
        {
            Domain.OrderedFilm filmModel = null;
            if (film != null)
            {
                filmModel = new Domain.OrderedFilm(film.Id, Map(film.Film));
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

        private Domain.OrderedFilmList Map(Persistence.OrderedList orderedFilmList)
        {
            var films = new List<Domain.OrderedFilm>();
            if (orderedFilmList?.OrderedFilms != null)
            {
                var mapping = orderedFilmList.OrderedFilms.ToDictionary(k => k, v => Map(v));
                foreach (var kv in mapping.Where(m => m.Key.LesserRankedFilmItems != null))
                {
                    foreach (var greater in kv.Key.LesserRankedFilmItems)
                    {
                        var domain = mapping[greater.GreaterRankedFilm];
                        kv.Value.HigherRankedObjects.Add(domain);
                    }
                }
                films.AddRange(mapping.Values);
            }
            var filmList = new Domain.OrderedFilmList(
                orderedFilmList.Id,
                orderedFilmList.Completed,
                films,
                null,
                null);
            return filmList;
        }
    }
}
