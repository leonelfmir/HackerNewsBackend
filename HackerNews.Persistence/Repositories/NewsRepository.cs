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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMemoryCache _memoryCache;
        
        private const string cacheKey = "news";
        private readonly string _maxItemUrl;
        private readonly string _getItemUrl;
        private readonly string _newIdsUrl;

        public NewsRepository(IConfiguration configuration, IHttpClientFactory httpClientFactory, IMemoryCache memoryCache)
        {
            _maxItemUrl = configuration["ApiUrls:maxItem"];
            _getItemUrl = configuration["ApiUrls:itemById"];
            _newIdsUrl = configuration["ApiUrls:newIds"];
            _httpClientFactory = httpClientFactory;
            _memoryCache = memoryCache;
        }

        public async Task<IEnumerable<New>> ListAsync()
        {
            return await GetNews();
        }

        public async Task<IEnumerable<New>> SearchByTitleAsync(string value)
        {
            var news = await GetNews();
            return news.Where(n => n != null).Where(n => n.Title.Contains(value, StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Updates cache to get new stories
        /// </summary>
        /// <returns></returns>
        private async Task UpdateCache()
        {
            var oldCache = await IsCacheOld();

            if(!oldCache.isOld)
            {
                return;
            }

            await AddNewItemsToCache(oldCache.maxItem, oldCache.lastItem);
        }

        private async Task AddNewItemsToCache(int maxItem, int lastItem)
        {
            var numberOfNewItems = maxItem - lastItem;
            var newsIds = Enumerable.Range(maxItem + 1, numberOfNewItems);
            var getNews = newsIds.Select(GetNewById);

            var newNews = await Task.WhenAll(getNews);
            var currentNews = await GetNews();

            var newList = currentNews.Concat(newNews);

            _memoryCache.Set(cacheKey, newList);
        }

        private async Task<(bool isOld, int lastItem, int maxItem)> IsCacheOld()
        {
            var maxItemTask = GetMaxItem();
            var newsTask = GetNews();

            await Task.WhenAll(maxItemTask, newsTask);

            var news = await newsTask;
            var maxItem = await maxItemTask;

            var lastItem = news.Max(n => n.Id);

            return (lastItem > maxItem, lastItem, maxItem);
        }

        private async Task<IEnumerable<New>> GetNews()
        {
            var res = await _memoryCache.GetOrCreateAsync(cacheKey, entry =>
            {
                return GetNewsFromApi();
            });

            return res;
        }

        private async Task<int> GetMaxItem()
        {
            using (var httpClient = _httpClientFactory.CreateClient())
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
            using (var httpClient = _httpClientFactory.CreateClient())
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
            using (var httpClient = _httpClientFactory.CreateClient())
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
