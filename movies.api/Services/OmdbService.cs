using System;
using Microsoft.AspNetCore.WebUtilities;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using movies.api.Interfaces;
using movies.api.DataTransferObjects;
using Newtonsoft.Json;

namespace movies.api.Services
{
    /// <summary>
    /// Communicates with the Omdb API
    /// http://www.omdbapi.com/
    /// </summary>
    public class OmdbService : IOmdbService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;

        public OmdbService(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _configuration = config;
        }

        /// <summary>
        /// Searches the Omdb API for a movie by title or part of the title.
        /// </summary>
        /// <param name="searchTerm"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MovieSearchResult>> SearchMoviesAsync(string searchTerm, string year)
        {
            try
            {
                var queryParams = new Dictionary<string, string>()
                {
                    { "s", searchTerm}
                };

                if (!string.IsNullOrEmpty(year))
                {
                    queryParams.Add("y", year);
                }

                queryParams = AddConfigQueryParams(queryParams);

                var client = _clientFactory.CreateClient("omdb");
                var response = await client.GetAsync(QueryHelpers.AddQueryString("", queryParams));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<OmdbResult>(content);
                    if (result.Response)
                    {
                        return result.Search;
                    }
                }

                return new List<MovieSearchResult>();
            }
            catch (Exception)
            {
                // todo log error and implement retry logic
                return new List<MovieSearchResult>();
            }
        }

        /// <summary>
        /// Searches the Omdb Api for a movie by the ImdbId
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns>(bool success, MovieAggregatedResult movie)</returns>
        public async Task<(bool success, MovieAggregatedResult movie)> TryGetMovieByImdbIdAsync(string imdbId)
        {
            try
            {
                var queryParams = AddConfigQueryParams(new Dictionary<string, string>() { { "i", imdbId } });

                var client = _clientFactory.CreateClient("omdb");

                var response = await client.GetAsync(QueryHelpers.AddQueryString("", queryParams));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var movie = JsonConvert.DeserializeObject<MovieAggregatedResult>(content);

                    return (true, movie);
                }

                return (false, default);
            }
            catch (Exception)
            {
                // todo add logging and implement retry logic
                return (false,default);
            }

        }

        private Dictionary<string, string> AddConfigQueryParams(Dictionary<string, string> queryParams)
        {
            queryParams.Add("apiKey", _configuration.GetSection("ApiKeys").GetSection("Omdb").GetSection("Key").Value);
            return queryParams;
        }
    }
}
