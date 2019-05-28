// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function () {
    $("#filmSearch").autocomplete({
        lookup: function (query, done) {
            $.ajax({
                url: `/api/search/films/${query}`,
                dataType: 'json',
                success: function (data) {
                    const suggestions = data.map(r => { return { "value": r.title, "data": r.tmdbId }; });
                    const result = { suggestions: suggestions };

                    done(result);
                }
            });
        },
        onSelect: function (suggestion) {
            $("#tmdbId").val(suggestion.data);
        }
    });
});