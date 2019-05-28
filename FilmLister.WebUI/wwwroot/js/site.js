// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

let currentSuggestions = [];
let selectedSuggestion = null;


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

    $("#filmForm").submit(function (event) {
        if (selectedSuggestion) {
            $("#tmdbId").val(selectedSuggestion.tmdbId);
        } else {
            event.preventDefault();
        }

    });
});
