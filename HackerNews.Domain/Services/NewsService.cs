using HackerNews.Domain.Communication;
using HackerNews.Domain.Models;
using HackerNews.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Domain.Services
{
    public class NewsService : INewsService
    {
        private readonly INewsRepository _newsRepository;
       

        public NewsService(INewsRepository newsRepository)
        {
            this._newsRepository = newsRepository;
        }


        public async Task<GenericResponse<IEnumerable<New>>> ListAsync()
        {
            try
            {
                var news = await _newsRepository.ListAsync();
                return new GenericResponse<IEnumerable<New>>(news);
            }
            catch (Exception ex)
            {
                // we can implement login here to capture the error
                return new GenericResponse<IEnumerable<New>>("Error loading news");
            }
        }


        public async Task<GenericResponse<IEnumerable<New>>> SearchAsync(string value)
        {
            try
            {
                var news = await _newsRepository.SearchByTitleAsync(value);
                return new GenericResponse<IEnumerable<New>>(news);
            }
            catch (Exception ex)
            {
                // we can implement login here to capture the error
                return new GenericResponse<IEnumerable<New>>("Error loading news");
            }
        }
    }
}
