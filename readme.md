# Film Lister

A website for listing and choosing favourite films.

## Deployment

The following command will deploy the database to localhost:
```
dotnet ef database update --project FilmLister.Persistence --startup-project FilmLister.WebUI
```
The connection string can be changed in FilmLister.Persistence/appsettings.json.

## Creating a Migration

After making changes run the following to create a migration. Run the update command to apply changes.
```
dotnet ef migrations add < name of migration > --project FilmLister.Persistence --startup-project FilmLister.WebUI
```
