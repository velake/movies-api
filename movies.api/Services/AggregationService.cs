using System.Collections.Generic;
using System.Threading.Tasks;
using movies.api.DataTransferObjects;
using movies.api.Interfaces;

namespace movies.api.Services
{
    public class AggregationService : IAggregationService
    {
        private readonly IOmdbService _omdbService;
        private readonly IYoutubeService _youtubeService;
        private readonly IGuideboxService _guideboxService;

        public AggregationService(IYoutubeService youtubeService, IOmdbService omdbService, IGuideboxService guideboxService)
        {
            _youtubeService = youtubeService;
            _omdbService = omdbService;
            _guideboxService = guideboxService;
        }

        public async Task<IEnumerable<MovieSearchResult>> SearchMoviesAsync(string searchTerm, string year)
        {
            var movies = new List<MovieSearchResult>();
            var result = await _omdbService.SearchMoviesAsync(searchTerm, year).ConfigureAwait(false);

            movies.AddRange(result);

            return movies;
        }

        public async Task<MovieAggregatedResult> AggregateMovieInformationAsync(string imdbId, bool includeSources = false)
        {
            var omdbMovieResult = await _omdbService.TryGetMovieByImdbIdAsync(imdbId);

            if (!omdbMovieResult.success) return default;

            var videoUrls = await _youtubeService.SearchTrailerAsync(omdbMovieResult.movie).ConfigureAwait(false);

            omdbMovieResult.movie.TrailerUrls = (List<string>)videoUrls;

            if (includeSources)
            {
                var sources = await _guideboxService.GetMovieStreamingSourcesAsync(imdbId);
                omdbMovieResult.movie.Sources = sources;
            }

            return omdbMovieResult.movie;
        }

    }
}
