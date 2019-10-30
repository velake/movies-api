using System.Collections.Generic;
using System.Threading.Tasks;
using movies.api.DataTransferObjects;

namespace movies.api.Interfaces
{
    public interface IYoutubeService
    {
        Task<IEnumerable<string>> SearchTrailerAsync(MovieAggregatedResult movie);
    }
}
