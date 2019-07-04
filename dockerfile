FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS builder

RUN apt-get update
RUN apt-get install -y unzip libunwind8 gettext
ADD . /FilmLister
WORKDIR /FilmLister
RUN ./cake.sh -target=PublishWebUIDocker

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

ARG tmdbApiKey
ENV envTmdbApiKey=$tmdbApiKey

COPY --from=builder /FilmLister/FilmLister.WebUI/bin/Release/netcoreapp2.2/publish/ /FilmLister/
WORKDIR /FilmLister
ENTRYPOINT dotnet FilmLister.WebUI.dll --TmdbApiKey $envTmdbApiKey
