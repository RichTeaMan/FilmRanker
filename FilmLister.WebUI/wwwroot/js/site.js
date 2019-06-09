// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/**
 * @typedef {Object} personSuggestion
 * @property {string} value
 * @property {string} title
 * @property {string} tmdbId
 * @property {string} imageUrl
 * @property {number} releaseYear
 * @property {string} job
 */

let currentSuggestions = [];
let selectedSuggestion = null;

/**
 * @type {personSuggestion[]}
 */
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

                    /**
                     * @type {personSuggestion[]}
                     */
                    const suggestions = data.map(r => {
                        const suffix = r.releaseYear ? ` (${r.releaseYear})` : '';
                        /**
                         * @type {personSuggestion}
                         */
                        const ps = {
                            "value": `${r.title}, ${r.jobTitle}${suffix}`,
                            "title": r.title,
                            "tmdbId": r.tmdbId,
                            "imageUrl": r.imageUrl,
                            "releaseYear": r.releaseYear,
                            "job": r.jobTitle
                        };
                        return ps;
                    });
                    personSuggestions = suggestions;
                    updateJobFilters();
                    $("#filmsPerPersonSearch").prop('disabled', false);
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

            const filteredJobs = new Set($('input.job-check').filter(function (_, el) {
                return $(el).is(':checked');
            }).map(function (_, el) {
                return $(el).val();
            }).get());

            const filteredSuggestions = personSuggestions.filter(function (item) {
                return item.value.toLowerCase().includes(query.toLowerCase()) && filteredJobs.has(item.job);
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

        const templateId = $("#filmListTemplateId").val();

        if (selectedSuggestion) {
            $.post({
                url: '/api/filmListTemplate/addFilm',
                data: { filmListTemplateId: templateId, tmdbId: selectedSuggestion.tmdbId },
            }).done(response => {
                $("#filmRow").children().remove();
                for (const film of response.films) {
                    $("#filmRow").append(`
                        <div class="col-4 col-md-2">
                            <figure>
                                <img class="img-fluid" src="${film.imageUrl}" />
                                <figcaption>
                                    ${film.displayName}
                                </figcaption>
                            </figure>
                        </div>`
                    );
                }

                selectedSuggestion = null;
                $("#filmSearch").val('');
                $("#filmsPerPersonSearch").val('');
                $("#selectedFilmImg").attr("src", "");
            }).fail(error => {
                alert(`Failed to submit choice: ${error.responseText}`)
            });
        }
        event.preventDefault();
    });
});

function updateJobFilters() {
    const jobs = new Set(personSuggestions.map(item => {
        return item.job
    }));

    $("#jobFilterContainer").children().remove();

    for (const job of jobs) {

        const jobContainer = $(document.createElement('span')).attr({
            class: 'form-check form-check-inline'
        });
        const label = $(document.createElement('label')).attr({
            class: 'switch control-label',
        });
        label.append(
            $(document.createElement('input')).attr({
                id: `${job}Chb`,
                class: 'form-check-input job-check',
                value: `${job}`,
                type: 'checkbox',
                checked: 'checked'
            }));
        label.append(`${job}`);
        jobContainer.append(label);

        $("#jobFilterContainer")
            .append(jobContainer);
    }

}
