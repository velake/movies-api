using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using movies.api.DataTransferObjects;
using movies.api.Interfaces;

namespace movies.api.Controllers
{
    [ApiConventionType(typeof(DefaultApiConventions))]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly IAggregationService _aggregationService;

        private readonly ILogger<MoviesController> _logger;

        public MoviesController(ILogger<MoviesController> logger, IAggregationService aggregationService)
        {
            _logger = logger;
            _aggregationService = aggregationService;
        }

        /// <summary>
        /// Searches for movies which include the given query
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Get /api/v1/movies/search?title=bambi
        /// </remarks>
        /// <param name="movieTitle"></param>
        /// <param name="year"></param>
        /// <returns>List of Movies</returns>
        /// <response code="200">Returns the List of Movies or empty list if nothing was found</response>
        /// <response code="404">If the title query does not contain a value</response>
        [Route("api/v1/movies/search")]
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ResponseCache(Duration = 86400, VaryByQueryKeys = new[] { "*" })]
        public async Task<ActionResult<IEnumerable<IMovie>>> GetMovieSearchResultsAsync(
            [FromQuery(Name = "title")] string movieTitle,
            [FromQuery(Name = "year")] string year = "")
        {
            if (string.IsNullOrEmpty(movieTitle))
            {
                return NotFound();
            }
            var result = await _aggregationService.SearchMoviesAsync(movieTitle, year);

            return result.ToList();
        }

        /// <summary>
        /// Returns a single movie for the provided imdbId
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     Get /api/v1/movies?imdbId=tt5519340&includeSources=true
        /// </remarks>
        /// <param name="imdbId"></param>
        /// <param name="includeSources"></param>
        /// <returns>A single movie matching the given imdbId</returns>
        /// <response code="200">Returns the matching movie</response>
        /// <response code="400">If the imdbId query does not contain a value</response>
        /// <response code="404">If there was no movie found with the given imdbId</response>
        [Route("api/v1/movies")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ResponseCache(Duration = 86400, VaryByQueryKeys = new[] { "*" })]
        [HttpGet]
        public async Task<ActionResult<MovieAggregatedResult>> GetMovieTrailerAsync(
            [FromQuery(Name = "imdbId")] string imdbId,
            [FromQuery(Name = "includeSources")] bool includeSources)
        {
            if (string.IsNullOrEmpty(imdbId))
                return BadRequest();

            var movie = await _aggregationService.AggregateMovieInformationAsync(imdbId, includeSources);
            if (movie == null)
            {
                return NotFound();
            }

            return Ok(movie);
        }
    }
}
