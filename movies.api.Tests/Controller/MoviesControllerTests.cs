using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using movies.api.Controllers;
using movies.api.DataTransferObjects;
using movies.api.Interfaces;
using Xunit;

namespace movies.api.Tests.Controller
{
    public class MoviesControllerTests
    {
        private MoviesController _moviesController;

        [Fact]
        public async Task GetMovieSearchResultsAsync_WithSearchParam_ReturnsResult()
        {
            var mockAggregationService = new Mock<IAggregationService>();
            var mockLogger = new Mock<ILogger<MoviesController>>();

            const string searchTerm = "Bambi";
            const string year = "";

            mockAggregationService.Setup(service =>
                service.SearchMoviesAsync(searchTerm, year)).ReturnsAsync(new List<MovieSearchResult>()
            {
                new MovieSearchResult()
                {
                    Title = "Bambi"
                }
            });

            _moviesController = new MoviesController(mockLogger.Object, mockAggregationService.Object);

            var result = await _moviesController.GetMovieSearchResultsAsync(searchTerm);

            Assert.NotNull(result);
            Assert.Equal(searchTerm, result.Value.First().Title);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task GetMovieSearchResultsAsync_NoSearchParam_ReturnsNotFound(string searchValue)
        {
            var mockAggregationService = new Mock<IAggregationService>();
            var mockLogger = new Mock<ILogger<MoviesController>>();

            _moviesController = new MoviesController(mockLogger.Object, mockAggregationService.Object);

            var result = await _moviesController.GetMovieSearchResultsAsync(searchValue);
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task GetMovieTrailerAsync_ValidQuery_ReturnsMovie()
        {
            var mockLogger = new Mock<ILogger<MoviesController>>();
            var mockAggregationService = new Mock<IAggregationService>();
            const string imdbId = "123";
            const bool includeSources = false;

            mockAggregationService.Setup(service =>
                service.AggregateMovieInformationAsync(imdbId, includeSources)).ReturnsAsync(new MovieAggregatedResult()
                {
                    ImdbId = imdbId
                });

            _moviesController = new MoviesController(mockLogger.Object, mockAggregationService.Object);

            var result = await _moviesController.GetMovieTrailerAsync(imdbId, includeSources);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetMovieTrailerAsync_ValidQuery_ReturnsMovieNotFound()
        {
            var mockLogger = new Mock<ILogger<MoviesController>>();
            var mockAggregationService = new Mock<IAggregationService>();
            const string imdbId = "123";
            const bool includeSources = false;

            mockAggregationService.Setup(service =>
                service.AggregateMovieInformationAsync(imdbId, includeSources)).Returns(Task.FromResult<MovieAggregatedResult>(null));

            _moviesController = new MoviesController(mockLogger.Object, mockAggregationService.Object);

            var result = await _moviesController.GetMovieTrailerAsync(imdbId, includeSources);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }
}
