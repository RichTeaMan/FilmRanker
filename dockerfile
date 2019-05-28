FROM microsoft/dotnet:2.2-sdk

ARG tmdbApiKey
ENV envTmdbApiKey=$tmdbApiKey

RUN apt-get update
RUN apt-get install -y unzip libunwind8 gettext
RUN mkdir DeathClock
ADD . /FilmLister
WORKDIR /FilmLister
RUN ./cake.sh -target=build
ENTRYPOINT ["./cake.sh", "-target=WebUIDocker", "--TmdbApiKey $envTmdbApiKey"]
CMD []
