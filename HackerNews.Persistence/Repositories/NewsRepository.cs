using HackerNews.Domain.Models;
using HackerNews.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Persistence.Repositories
{
    public class NewsRepository : INewsRepository
    {
        private New TestNew = new New
        {
            By = "a",
            Id = 1,
            Time = DateTime.Now,
            Title = "t",
            Type = "ty",
            Url = "url"
        };
        public async Task<IEnumerable<New>> ListAsync()
        {
            return new List<New> { TestNew };

        }

        public async Task<IEnumerable<New>> SearchByTitleAsync(string value)
        {
            return new List<New> { TestNew };

        }
    }
}
