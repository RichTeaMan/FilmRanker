using FilmLister.Domain;
using FilmLister.Persistence;
using FilmLister.Service.Exceptions;
using FilmLister.TmdbIntegration;
using FilmLister.TmdbIntegration.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RichTea.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FilmLister.Service
{
    public class FilmService
    {
        private readonly ILogger logger;

        private readonly OrderService orderService;

        private readonly TmdbService tmdbService;

        private readonly FilmListerContext filmListerContext;

        public FilmService(ILogger<FilmService> logger, OrderService orderService, TmdbService tmdbService, FilmListerContext filmListerContext)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            var filmListTemplates = await filmListerContext.FilmListTemplates
                .Include(l => l.FilmListItems)
                    .ThenInclude(i => i.Film)
                .Where(l => l.FilmListItems != null && l.FilmListItems.Any())
                .Where(l => l.Published)
                .ToArrayAsync();

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

            var films = persistenceListTemplate.FilmListItems.Select(i => new Persistence.OrderedFilm() { Film = i.Film })
                .RandomiseOrder()
                .ToList();
            var filmList = new Persistence.OrderedList()
            {
                OrderedFilms = films,
                FilmListTemplate = persistenceListTemplate,
                StartDateTime = DateTimeOffset.UtcNow
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
                }
            }
            else
            {
                throw new ListNotFoundException(id);
            }
        }

        public async Task SubmitIgnoreFilm(int id, int filmId)
        {
            var orderedFilmList = await filmListerContext.OrderedLists
                .Include(ol => ol.OrderedFilms)
                    .ThenInclude(f => f.Film)
                .FirstOrDefaultAsync(ol => ol.Id == id);
            if (orderedFilmList != null)
            {
                var filmToIgnore = orderedFilmList.OrderedFilms.FirstOrDefault(f => f.Id == filmId);

                if (filmToIgnore == null)
                {
                    throw new FilmNotFoundException(filmId);
                }
                else
                {
                    filmToIgnore.Ignored = true;
                    await filmListerContext.SaveChangesAsync();
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

            Domain.OrderedFilmList filmList;
            if (persistenceOrderedFilmList.Completed)
            {
                filmList = MapCompletedList(persistenceOrderedFilmList);
            }
            else
            {
                var orderedFilmList = Map(persistenceOrderedFilmList);
                var orderResult = orderService.OrderFilms(orderedFilmList.SortedFilms.Where(f => !f.Ignore));

                if (orderResult.Completed)
                {
                    persistenceOrderedFilmList.Completed = orderResult.Completed;
                    persistenceOrderedFilmList.CompletedDateTime = DateTimeOffset.UtcNow;

                    var sortedFilms = orderResult.SortedResults.ToArray();
                    for (int i = 0; i < sortedFilms.Length; i++)
                    {
                        var film = sortedFilms[i].Film;
                        var persistenceFilm = persistenceOrderedFilmList.OrderedFilms.First(f => f.Film.Id == film.Id);
                        persistenceFilm.Ordinal = i;
                    }

                    await filmListerContext.SaveChangesAsync();

                    filmList = MapCompletedList(persistenceOrderedFilmList);
                }
                else
                {
                    filmList = new Domain.OrderedFilmList(
                        orderedFilmList.Id,
                        orderResult.Completed,
                        orderResult.SortedResults.ToArray(),
                        persistenceOrderedFilmList.OrderedFilms.Where(f => f.Ignored).Select(f => Map(f)).ToArray(),
                        orderResult.LeftSort,
                        orderResult.RightSort);
                }
            }

            return filmList;
        }

        public async Task UpdateAllFilmsFromTmdb()
        {
            foreach (var film in filmListerContext.Films)
            {
                await UpdateFilmFromTmdb(film);
            }
            await filmListerContext.SaveChangesAsync();
        }

        public async Task UpdateFilmFromTmdb(Persistence.Film film)
        {
            var movieDetail = await tmdbService.FetchMovieDetails(film.TmdbId);
            var movieCredits = await tmdbService.FetchMovieCredits(film.TmdbId);

            var director = movieCredits.FetchDirectorJobs().FirstOrDefault();

            film.Name = movieDetail.Title;
            film.ImageUrl = CreateFullImagePath(movieDetail.PosterPath);
            film.ImdbId = movieDetail.ImdbId;
            film.Budget = movieDetail.Budget != 0 ? (long?)movieDetail.Budget : null;
            film.Revenue = movieDetail.Revenue != 0 ? (long?)movieDetail.Revenue : null;
            film.Director = director?.Name;
            film.ReleaseDate = movieDetail.ReleaseDate;
            film.VoteAverage = movieDetail.VoteAverage;
        }

        public async Task<Domain.Film> RetrieveFilmByTmdbId(int tmdbId)
        {
            var film = await filmListerContext.Films.FirstOrDefaultAsync(f => f.TmdbId == tmdbId);
            if (film == null)
            {
                film = new Persistence.Film
                {
                    TmdbId = tmdbId,
                };

                // try tmdb
                await UpdateFilmFromTmdb(film);

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

        public async Task DeleteFilmListTemplateById(int id)
        {
            var list = await filmListerContext.FilmListTemplates
                .Include(l => l.FilmListItems)
                    .ThenInclude(i => i.Film)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (list != null)
            {
                var derivedLists = await filmListerContext
                    .OrderedLists
                    .Include(l => l.OrderedFilms)
                        .ThenInclude(f => f.GreaterRankedFilmItems)
                    .Include(l => l.OrderedFilms)
                        .ThenInclude(f => f.LesserRankedFilmItems)
                    .Where(l => l.FilmListTemplate == list)
                    .ToArrayAsync();

                var derivedFilms = derivedLists
                    .Where(l => l.OrderedFilms != null)
                    .SelectMany(l => l.OrderedFilms)
                    .ToArray();

                var derivedRankings = derivedFilms
                    .SelectMany(f => f.GreaterRankedFilmItems)
                    .Union(derivedFilms
                    .SelectMany(f => f.LesserRankedFilmItems))
                    .ToArray();

                filmListerContext.OrderedFilmRankItems.RemoveRange(derivedRankings);
                filmListerContext.OrderedFilms.RemoveRange(derivedFilms);
                filmListerContext.OrderedLists.RemoveRange(derivedLists);
                filmListerContext.FilmListItems.RemoveRange(list.FilmListItems);
                filmListerContext.FilmListTemplates.Remove(list);

                await filmListerContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Adds a film to the film list template.
        /// </summary>
        /// <param name="filmListTemplate"></param>
        /// <param name="film"></param>
        /// <exception cref="FilmAlreadyInFilmListTemplateException">Occurs when the given film is already in the list.</exception>
        /// <returns></returns>
        public async Task<Domain.FilmListTemplate> AddFilmToFilmListTemplate(Domain.FilmListTemplate filmListTemplate, Domain.Film film)
        {
            var persistenceList = await filmListerContext.FilmListTemplates.FirstAsync(l => l.Id == filmListTemplate.Id);

            if (persistenceList.FilmListItems == null)
            {
                persistenceList.FilmListItems = new List<FilmListItem>();
            }
            if (persistenceList.FilmListItems.Any(item => item.Film.Id == film.Id))
            {
                throw new FilmAlreadyInFilmListTemplateException(film.Id, filmListTemplate.Id);
            }
            if (persistenceList.Published)
            {
                throw new FilmListTemplatePublishedException(filmListTemplate.Id);
            }

            var persistenceFilm = await filmListerContext.Films.FirstAsync(l => l.Id == film.Id);
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
        /// Creates a new empty film list with a generated name.
        /// </summary>
        /// <returns></returns>
        public async Task<Domain.FilmListTemplate> CreateFilmListTemplate()
        {
            var filmListTemplate = new Persistence.FilmListTemplate()
            {
                Name = $"New List - {DateTimeOffset.Now.Date.ToShortDateString()}"
            };
            await filmListerContext.FilmListTemplates.AddAsync(filmListTemplate);
            await filmListerContext.SaveChangesAsync();

            var domain = await RetrieveFilmListTemplateById(filmListTemplate.Id);
            return domain;
        }

        public async Task PublishFilmListTemplate(int filmListTemplateId)
        {
            var filmListTemplate = await filmListerContext
                .FilmListTemplates
                .FirstOrDefaultAsync(l => l.Id == filmListTemplateId);

            if (filmListTemplate == null)
            {
                throw new ListNotFoundException(filmListTemplateId);
            }

            filmListTemplate.Published = true;
            await filmListerContext.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the film with the given ID from the film list template.
        /// </summary>
        /// <param name="filmListTemplateId">Film list template ID.</param>
        /// <param name="tmdbId"></param>
        /// <exception cref="ListNotFoundException">Could not find list with the given ID.</exception>
        /// <returns></returns>
        public async Task RemoveFilmFromFilmListTemplate(int filmListTemplateId, int tmdbId)
        {
            var persistenceList = await filmListerContext.FilmListTemplates
                .Include(l => l.FilmListItems)
                    .ThenInclude(f => f.Film)
                .FirstOrDefaultAsync(l => l.Id == filmListTemplateId);
            if (persistenceList == null)
            {
                throw new ListNotFoundException(filmListTemplateId);
            }
            if (persistenceList.Published)
            {
                throw new FilmListTemplatePublishedException(filmListTemplateId);
            }

            persistenceList.FilmListItems.RemoveAll(f => f.Film.TmdbId == tmdbId);
            await filmListerContext.SaveChangesAsync();
        }

        /// <summary>
        /// Renames the film list template with the given ID.
        /// </summary>
        /// <param name="filmListId"></param>
        /// <param name="newListName"></param>
        /// <exception cref="ArgumentException">If new list name is null or longer than the maximum length.</exception>
        /// <exception cref="ListNotFoundException">Could not find list with the given ID.</exception>
        /// <returns></returns>
        public async Task RenameFilmListTemplate(int filmListId, string newListName)
        {
            if (string.IsNullOrEmpty(newListName) || newListName.Length >= Persistence.Constants.FILM_LIST_TEMPLATE_MAX_LENGTH)
            {
                throw new ArgumentException($"NewListName must be not null and have a maximum of{Persistence.Constants.FILM_LIST_TEMPLATE_MAX_LENGTH} characters.");
            }
            var persistenceList = await filmListerContext.FilmListTemplates.FirstOrDefaultAsync(l => l.Id == filmListId);
            if (persistenceList == null)
            {
                throw new ListNotFoundException(filmListId);
            }
            if (persistenceList.Published)
            {
                throw new FilmListTemplatePublishedException(filmListId);
            }

            persistenceList.Name = newListName;
            await filmListerContext.SaveChangesAsync();
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
            var titles = searchResult.Results
                .Select(r => new FilmTitle(r.Id, r.Title, CreateFullImagePath(r.PosterPath), r.ReleaseDate?.Year))
                .ToArray();
            return titles;
        }

        public async Task<Domain.Person[]> SearchPersons(string query)
        {
            var peopleResults = await tmdbService.SearchPeople(query);
            var result = peopleResults.Results.Select(r => Map(r)).ToArray();
            return result;
        }

        public async Task<FilmTitleWithPersonCredit[]> FetchFilmTitlesByPersonId(int tmdbId)
        {
            var titles = new List<FilmTitleWithPersonCredit>();
            var credit = await tmdbService.FetchPersonMovieCredits(tmdbId);
            foreach (var r in credit.Persons)
            {
                titles.Add(new FilmTitleWithPersonCredit(
                    r.Id,
                    r.Title,
                    CreateFullImagePath(r.PosterPath),
                    r.ReleaseDate?.Year,
                    tmdbId,
                    r.Job));
            }

            return titles.ToArray();
        }

        private Domain.Person Map(TmdbIntegration.Models.Person person)
        {
            Domain.Person personDomain = null;
            if (person != null)
            {
                personDomain = new Domain.Person(person.Id, person.Title);
            }
            return personDomain;
        }


        private Domain.Person Map(PersonSearchResult personSearchResult)
        {
            Domain.Person personDomain = null;
            if (personSearchResult != null)
            {
                personDomain = new Domain.Person(personSearchResult.Id, personSearchResult.Name);
            }
            return personDomain;
        }

        private Domain.Film Map(Persistence.Film film)
        {
            Domain.Film filmModel = null;
            if (film != null)
            {
                filmModel = new Domain.Film(
                   film.Id,
                   film.Name,
                   film.TmdbId,
                   film.ImageUrl,
                   film.ImdbId,
                   film.ReleaseDate?.Year,
                   film.Budget,
                   film.Revenue,
                   film.VoteAverage,
                   film.Director);
            }
            return filmModel;
        }

        private Domain.OrderedFilm Map(Persistence.OrderedFilm film)
        {
            Domain.OrderedFilm filmModel = null;
            if (film != null)
            {
                filmModel = new Domain.OrderedFilm(film.Id, film.Ignored, Map(film.Film));
            }
            return filmModel;
        }

        private Domain.OrderedFilmList MapCompletedList(OrderedList persistenceOrderedFilmList)
        {
            var orderedFilmsDictionary = persistenceOrderedFilmList
                .OrderedFilms
                .OrderBy(f => f.Ordinal)
                .ToDictionary(k => k, v => Map(v));

            var orderedFilms = new List<Domain.OrderedFilm>();

            foreach(var kvFilm in orderedFilmsDictionary.Where(kv => !kv.Key.Ignored))
            {
                var persistenceFilm = kvFilm.Key;
                var domainFilm = kvFilm.Value;

                if (persistenceFilm.GreaterRankedFilmItems?.Any() == true)
                {
                    try
                    {
                        var lesserRanked = persistenceFilm
                            .GreaterRankedFilmItems
                            .Where(f => f.GreaterRankedFilm == persistenceFilm)
                            .Select(f => orderedFilmsDictionary[f.LesserRankedFilm])
                            .Distinct()
                            .ToArray();

                        domainFilm.AddLesserRankedFilms(lesserRanked);
                    }
                    catch(Exception ex)
                    {
                        logger.LogError(ex, "Error occurred mapping completed list.");
                    }
                }
                orderedFilms.Add(kvFilm.Value);
            }

            var filmList = new Domain.OrderedFilmList(
                    persistenceOrderedFilmList.Id,
                    persistenceOrderedFilmList.Completed,
                    orderedFilms.ToArray(),
                    persistenceOrderedFilmList.OrderedFilms.Where(f => f.Ignored).Select(f => Map(f)).ToArray(),
                    null,
                    null);

            return filmList;
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
                films.ToArray(),
                orderedFilmList.Published);
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
                        kv.Value.AddHigherRankedObject(domain);
                    }
                }

                films.AddRange(mapping.Values);
            }
            var filmList = new Domain.OrderedFilmList(
                orderedFilmList.Id,
                orderedFilmList.Completed,
                films,
                new Domain.OrderedFilm[0],
                null,
                null);
            return filmList;
        }
    }
}
