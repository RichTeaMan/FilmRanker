// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

let currentSuggestions = [];
let selectedSuggestion = null;
let personSuggestions = [];


$(document).ready(function () {
    $("#filmSearch").autocomplete({
        showNoSuggestionNotice: true,
        lookup: function (query, done) {
            $.ajax({
                url: `/api/search/films/${query}`,
                dataType: 'json',
                success: function (data) {
                    const suggestions = data.map(r => {
                        return {
                            "value": r.releaseYear ? `${r.title} (${r.releaseYear})` : r.title,
                            "title": r.title,
                            "tmdbId": r.tmdbId,
                            "imageUrl": r.imageUrl,
                            "releaseYear": r.releaseYear
                        };
                    });
                    const result = { suggestions: suggestions };
                    currentSuggestions = suggestions;

                    done(result);
                }
            });
        },
        onSelect: function (suggestion) {
            selectedSuggestion = suggestion;
            $("#selectedFilmImg").attr("src", selectedSuggestion.imageUrl);
        },
        onInvalidateSelection: function () {
            selectedSuggestion = null;
            $("#selectedFilmImg").attr("src", "");
        },
        onHide: function (container) {
        }
    });

    $("#personSearch").autocomplete({
        showNoSuggestionNotice: true,
        lookup: function (query, done) {
            $.ajax({
                url: `/api/search/persons/${query}`,
                dataType: 'json',
                success: function (data) {
                    const suggestions = data.map(r => {
                        return {
                            "value": `${r.name}`,
                            "tmdbId": r.tmdbId
                        };
                    });
                    const result = { suggestions: suggestions };
                    currentSuggestions = suggestions;
                    done(result);
                }
            });
        },
        onSelect: function (suggestion) {

            $.ajax({
                url: `/api/search/filmsByPersonId/${suggestion.tmdbId}`,
                dataType: 'json',
                success: function (data) {
                    const suggestions = data.map(r => {
                        const suffix = r.releaseYear ? ` (${r.releaseYear})` : '';
                        return {
                            "value": `${r.title}, ${r.jobTitle}${suffix}`,
                            "title": r.title,
                            "tmdbId": r.tmdbId,
                            "imageUrl": r.imageUrl,
                            "releaseYear": r.releaseYear,
                            "job": r.jobTitle
                        };
                    });
                    personSuggestions = suggestions;
                    $("#filmsPerPersonSearch").prop('disabled', false);
                    console.log(suggestions);
                }
            });
        },
        onInvalidateSelection: function () {
            $("#filmsPerPersonSearch").prop('disabled', true);
        },
        onHide: function (container) {
        }
    });

    $("#filmsPerPersonSearch").autocomplete({
        showNoSuggestionNotice: true,
        minChars: 0,
        lookup: function (query, done) {
            const filteredSuggestions = personSuggestions.filter(function (item) {
                return item.value.toLowerCase().includes(query.toLowerCase());
            });
            const result = { suggestions: filteredSuggestions };
            done(result);
        },
        onSelect: function (suggestion) {
            selectedSuggestion = suggestion;
            $("#selectedFilmImg").attr("src", selectedSuggestion.imageUrl);
        },
        onInvalidateSelection: function () {
            selectedSuggestion = null;
            $("#selectedFilmImg").attr("src", "");
        },
        onHide: function (container) {
        }
    });

    $("#filmForm").submit(function (event) {
        if (selectedSuggestion) {
            $("#tmdbId").val(selectedSuggestion.tmdbId);
        } else {
            event.preventDefault();
        }
    });
});
