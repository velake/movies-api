using movies.api.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace movies.api.Interfaces
{
    /// <summary>
    /// Aggregates Results from different Sources
    /// </summary>
    public interface IAggregationService
    {
        /// <summary>
        ///  Searches for movies by part of the title and year
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="year"></param>
        /// <returns>a list of movies</returns>
        Task<IEnumerable<MovieSearchResult>> SearchMoviesAsync(string searchTerm, string year);

        /// <summary>
        /// Aggregates data from different sources 
        /// </summary>
        /// <param name="imdbId"></param>
        /// <param name="includeSources"></param>
        /// <returns>a movie object</returns>
        Task<MovieAggregatedResult> AggregateMovieInformationAsync(string imdbId, bool includeSources = false);
    }
}
