# Film Lister

A website for listing and determining your favourite films mathematically. A live version can be found at https://filmlister.richteaman.com/.

The project uses ASP NET Core to host a website that presents a pair of film choices. These choices are then stored
in a SQL database that can be eventually be used to order film preferences via the [quicksort algorithm](https://en.wikipedia.org/wiki/Quicksort).

Film information, such as titles and posters, are provided via [The Movie Database](https://www.themoviedb.org/).

## Web App Deployment

The application can be deployed with Docker. You'll need a [TMDB API key](https://developers.themoviedb.org/3/getting-started/introduction) for full functionality.
```bash
docker build -t film-lister --build-arg tmdb=YOUR_TMDB_API_KEY .
docker run -d --name film-lister -p 8080:80 film-lister
```

## Database Deployment

The following command will deploy the database to localhost:
```bash
dotnet ef database update --project FilmLister.Persistence --startup-project FilmLister.WebUI
```
The connection string can be changed in FilmLister.Persistence/appsettings.json.

## Creating a Migration

After making changes run the following to create a migration. Run the update command to apply changes.
```bash
dotnet ef migrations add < name of migration > --project FilmLister.Persistence --startup-project FilmLister.WebUI
```
