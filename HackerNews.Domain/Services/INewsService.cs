using HackerNews.Domain.Communication;
using HackerNews.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Domain.Services
{
    public interface INewsService
    {
        /// <summary>
        /// Get all news
        /// </summary>
        /// <returns></returns>
        Task<GenericResponse<IEnumerable<New>>> ListAsync();

        /// <summary>
        /// Search news by value
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<GenericResponse<IEnumerable<New>>> SearchAsync(string value);
    }
}
