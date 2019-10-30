using System.Collections.Generic;

namespace movies.api.DataTransferObjects
{
    public class OmdbResult
    {
        public int TotalResults { get; set; }

        public bool Response { get; set; }

        public IEnumerable<MovieSearchResult> Search { get; set; }
    }
}
