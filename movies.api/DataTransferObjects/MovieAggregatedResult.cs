using System.Collections.Generic;
using movies.api.Interfaces;

namespace movies.api.DataTransferObjects
{
    public class MovieAggregatedResult : IMovie
    {
        public string Title { get; set; }

        public string Year { get; set; }

        public string ImdbId { get; set; }

        public string Poster { get; set; }

        public List<string> TrailerUrls { get; set; }

        public string ImdbRating { get; set; }

        public string Plot { get; set; }

        public IEnumerable<MovieStreamingSource> Sources { get; set; }
    }
}
