using System.Collections.Generic;
using System.Threading.Tasks;
using movies.api.DataTransferObjects;

namespace movies.api.Interfaces
{
    public interface IGuideboxService
    {
        /// <summary>
        /// Gets subscription web based streaming sources for a movie 
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        Task<IEnumerable<MovieStreamingSource>> GetMovieStreamingSourcesAsync(string imdbId);

        /// <summary>
        /// Searches for a movie on the Guidebox Api to retrieve the Guidebox Movie Id
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        Task<(bool success, GuideboxMovieResult movie)> TryGetMovieByImdbIdAsync(string imdbId);
    }
}
