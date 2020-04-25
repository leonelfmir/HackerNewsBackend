using HackerNews.Domain.Models;
using HackerNews.Domain.Repositories;
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
            var newsIds = await GetNewsIds();
            var getNews = newsIds.Select(GetNewById);

            var news = await Task.WhenAll(getNews);
            return news;
        }

        private async Task<IEnumerable<int>> GetNewsIds()
        {
            const string apiUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(apiUrl))
                {
                    var news = await response.Content.ReadAsStringAsync();
                    var lst = JsonConvert.DeserializeObject<List<int>>(news);
                    return lst;
                }
            }
        }
        
        private async Task<New> GetNewById(int id)
        {
            var apiUrl = $"https://hacker-news.firebaseio.com/v0/item/{id}.json";
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
