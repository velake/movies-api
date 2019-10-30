using movies.api.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace movies.api.Interfaces
{
    public interface IOmdbService
    {
        Task<IEnumerable<MovieSearchResult>> SearchMoviesAsync(string searchTerm, string year);

        Task<(bool success, MovieAggregatedResult movie)> TryGetMovieByImdbIdAsync(string searchTerm);
    }
}
