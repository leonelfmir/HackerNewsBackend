using HackerNews.Domain.Models;
using HackerNews.Domain.Repositories;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Persistence.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private readonly IMemoryCache _memoryCache;

        private readonly string _maxItemUrl;
        private readonly string _getItemUrl;
        private readonly string _newIdsUrl;

        public NewsRepository(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _maxItemUrl = configuration["ApiUrls:maxItem"];
            _getItemUrl = configuration["ApiUrls:itemById"];
            _newIdsUrl = configuration["ApiUrls:newIds"];
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<New>> ListAsync()
        {
            return await GetNews();
        }

        public async Task<IEnumerable<New>> SearchByTitleAsync(string value)
        {
            return new List<New>();

        }

        private async Task<IEnumerable<New>> GetNews()
        {
            var res = await _memoryCache.GetOrCreateAsync("news", entry =>
            {
                return GetNewsFromApi();
            });

            return res;
        }

        private async Task<int> GetMaxItem()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_maxItemUrl))
                {
                    var maxItemsResult = await response.Content.ReadAsStringAsync();
                    var maxItem = JsonConvert.DeserializeObject<int>(maxItemsResult);
                    return maxItem;
                }
            }
        }

        private async Task<IEnumerable<New>> GetNewsFromApi()
        {
            var newsIds = await GetNewsIds();
            var getNews = newsIds.Select(GetNewById);

            var news = await Task.WhenAll(getNews);
            return news;
        }

        private async Task<IEnumerable<int>> GetNewsIds()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(_newIdsUrl))
                {
                    var news = await response.Content.ReadAsStringAsync();
                    var lst = JsonConvert.DeserializeObject<List<int>>(news);
                    return lst;
                }
            }
        }

        private async Task<New> GetNewById(int id)
        {
            var apiUrl = string.Format(_getItemUrl, id);
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiUrl))
                {
                    var newResult = await response.Content.ReadAsStringAsync();
                    var newObject = JsonConvert.DeserializeObject<New>(newResult);
                    return newObject;
                }
            }
        }
    }
}
