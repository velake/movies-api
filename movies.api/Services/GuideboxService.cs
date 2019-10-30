using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using movies.api.DataTransferObjects;
using movies.api.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace movies.api.Services
{
    /// <summary>
    /// Interacts with the Guidebox Api https://www.guidebox.com/docs#sources
    /// Used to enrich a MovieAggregatedResult with streaming sources
    /// </summary>
    public class GuideboxService : IGuideboxService
    {
        private IHttpClientFactory _clientFactory;
        private IConfiguration _configuration;

        public GuideboxService(IHttpClientFactory clientFactory, IConfiguration config)
        {
            _clientFactory = clientFactory;
            _configuration = config;
        }

        /// <summary>
        /// Requests a movie from the Guidebox Api to retrieve streaming sources. Currently only filters subscription_web_sources 
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MovieStreamingSource>> GetMovieStreamingSourcesAsync(string imdbId)
        {
            // Get single movie from Guidebox to retrieve the id
            var (success, movie) = await TryGetMovieByImdbIdAsync(imdbId);

            if (!success) return new List<MovieStreamingSource>();

            try
            {
                var queryParams = AddConfigQueryParams(new Dictionary<string, string>()
                {
                    { "sources", "subscription"}
                });

                var client = _clientFactory.CreateClient("guidebox");

                var response = await client.GetAsync(QueryHelpers.AddQueryString($"movies/{movie.Id}", queryParams));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(content);

                    if (json["subscription_web_sources"].Any())
                    {
                        var webSources = json["subscription_web_sources"].Children().ToList();

                        return webSources.Select(source =>
                        {
                            var dynamicSource = JsonConvert.DeserializeObject<dynamic>(source.ToString());
                            return new MovieStreamingSource()
                            {
                                DisplayName = dynamicSource.display_name,
                                Link = dynamicSource.link,
                                Source = dynamicSource.source,
                                ImdbId = imdbId
                            };
                        });
                    }
                }
                return new List<MovieStreamingSource>();

            }
            catch (Exception)
            {
                // todo log error and implement retry logic
                return new List<MovieStreamingSource>();

            }
        }

        public async Task<(bool success, GuideboxMovieResult movie)> TryGetMovieByImdbIdAsync(string imdbId)
        {
            try
            {
                var queryParams = AddConfigQueryParams(new Dictionary<string, string>()
                {
                    { "field", "id"},
                    { "id_type", "imdb" },
                    { "type", "movie" },
                    { "query", imdbId }
                });

                var client = _clientFactory.CreateClient("guidebox");

                var response = await client.GetAsync(QueryHelpers.AddQueryString("search", queryParams));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var movie = JsonConvert.DeserializeObject<GuideboxMovieResult>(content);

                    return movie.Id == null ? (false,  default) : (true, movie);
                }

                return (false, default);
            }
            catch (Exception)
            {
                // todo log error and implement retry logic
                return (false, default);
            }
        }

        // todo move to Helper-method with type as parameter
        private IDictionary<string, string> AddConfigQueryParams(Dictionary<string, string> queryParams)
        {
            queryParams.Add("api_key", _configuration.GetSection("ApiKeys").GetSection("Guidebox").GetSection("Key").Value);
            return queryParams;
        }

    }
}
