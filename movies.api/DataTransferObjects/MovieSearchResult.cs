using movies.api.Interfaces;

namespace movies.api.DataTransferObjects
{
    public class MovieSearchResult : IMovie
    {
        public string Title { get; set; }

        public string Year { get; set; }

        public string Poster { get; set; }

        public string ImdbId { get; set; }
    }
}


