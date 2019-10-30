using System;
using System.Collections.Generic;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using movies.api.DataTransferObjects;
using movies.api.Interfaces;

namespace movies.api.Services
{
    public class YoutubeService : IYoutubeService
    {
        private readonly IConfiguration _configuration;

        public YoutubeService(IConfiguration config)
        {
            _configuration = config;
        }

        public async Task<IEnumerable<string>> SearchTrailerAsync(MovieAggregatedResult movie)
        {
            if (string.IsNullOrEmpty(movie.Title))
            {
                return new List<string>();
            }

            try
            {
                var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                {
                    ApiKey = _configuration.GetSection("ApiKeys").GetSection("Youtube").GetSection("Key").Value,
                    ApplicationName = GetType().ToString()
                });

                var searchListRequest = youtubeService.Search.List("snippet");
                searchListRequest.Type = "video";
                searchListRequest.Q = $"{movie.Title} Movie Trailer {movie.Year ?? ""}";
                searchListRequest.MaxResults = 10;

                var searchListResponse = await searchListRequest.ExecuteAsync();
                var result = TransformSearchListResponse(searchListResponse);

                return result;
            }
            catch (Exception)
            {
                // todo log error and add retry logic
                return new List<string>();
            }

        }

        private static IEnumerable<string> TransformSearchListResponse(SearchListResponse searchListResponse)
        {
            var result = searchListResponse.Items.Select(item => $"https://www.youtube.com/embed/{item.Id.VideoId}").ToList();
            return result;
        }
    }
}
