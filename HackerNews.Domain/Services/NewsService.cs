using HackerNews.Domain.Communication;
using HackerNews.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Domain.Services
{
    public class NewsService : INewsService
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


        public async Task<GenericResponse<IEnumerable<New>>> ListAsync()
        {
            return new GenericResponse<IEnumerable<New>>( new List<New> { TestNew });
        }


        public async Task<GenericResponse<IEnumerable<New>>> SearchAsync(string value)
        {
            return new GenericResponse<IEnumerable<New>>( new List<New> { TestNew });
        }
    }
}
