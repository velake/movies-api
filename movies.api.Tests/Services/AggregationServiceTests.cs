using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using movies.api.DataTransferObjects;
using movies.api.Interfaces;
using movies.api.Services;
using Xunit;

namespace movies.api.Tests.Services
{
    public class AggregationServiceTests
    {
        private AggregationService _aggregationService;

        [Fact]
        public async Task SearchMoviesAsync_ValidInput_ReturnsMovieList_()
        {
            var mockOmbdService = new Mock<IOmdbService>();
            var mockYoutubeService = new Mock<IYoutubeService>();
            var guideBoxService = new Mock<IGuideboxService>();
            const string searchTerm = "movie";
            const string year = "1994";
            mockOmbdService.Setup(service =>
                service.SearchMoviesAsync(searchTerm, year)).ReturnsAsync(new List<MovieSearchResult>() { new MovieSearchResult() { Year = year, Title = searchTerm } });

            _aggregationService = new AggregationService(mockYoutubeService.Object, mockOmbdService.Object, guideBoxService.Object);
            var result = await _aggregationService.SearchMoviesAsync(searchTerm, year);

            Assert.NotNull(result);
            Assert.Equal(searchTerm, result.First().Title);
        }

        [Fact]
        public async Task AggregateMovieInformationAsync_MovieNotFound_ReturnsDefault()
        {
            var mockOmbdService = new Mock<IOmdbService>();
            var mockYoutubeService = new Mock<IYoutubeService>();
            var guideBoxService = new Mock<IGuideboxService>();

            const string imdbId = "1234";
            const bool includeSources = true;

            mockOmbdService.Setup(service =>
                service.TryGetMovieByImdbIdAsync(imdbId)).ReturnsAsync((false, default));

            _aggregationService = new AggregationService(mockYoutubeService.Object, mockOmbdService.Object, guideBoxService.Object);
            var result = await _aggregationService.AggregateMovieInformationAsync(imdbId, includeSources);

            Assert.Equal(default, result);
        }

        [Fact]
        public async Task AggregateMovieInformationAsync_MovieFound_ReturnsMovieWithSources()
        {
            var mockOmbdService = new Mock<IOmdbService>();
            var mockYoutubeService = new Mock<IYoutubeService>();
            var mockGuideBoxService = new Mock<IGuideboxService>();

            const string imdbId = "1234";
            const bool includeSources = true;
            const string sourceDisplayName = "streamingSource";

            mockOmbdService.Setup(service =>
                service.TryGetMovieByImdbIdAsync(imdbId)).ReturnsAsync((true, new MovieAggregatedResult() { ImdbId = imdbId }));

            mockYoutubeService.Setup(service =>
                service.SearchTrailerAsync(It.IsAny<MovieAggregatedResult>())).ReturnsAsync(new List<string>() { "trailer1" });

            mockGuideBoxService.Setup(service =>
                service.GetMovieStreamingSourcesAsync(imdbId)).ReturnsAsync(new List<MovieStreamingSource>() {
                    new MovieStreamingSource()
                    {
                        DisplayName = "streamingSource", Link = "link", Source = "source"
                    }}
                );

            _aggregationService = new AggregationService(mockYoutubeService.Object, mockOmbdService.Object, mockGuideBoxService.Object);
            var result = await _aggregationService.AggregateMovieInformationAsync(imdbId, includeSources);

            Assert.NotNull(result);
            Assert.IsType<MovieAggregatedResult>(result);
            Assert.Equal(sourceDisplayName, result.Sources.First().DisplayName);

        }
    }
}