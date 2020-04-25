using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using HackerNews.API.Filters;
using HackerNews.API.Resources;
using HackerNews.Domain.Models;
using HackerNews.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly INewsService _newsService;

        public NewsController(IMapper mapper, INewsService newsService)
        {
            _mapper = mapper;
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var news = await _newsService.ListAsync();

            if (!news.Success)
            {
                return BadRequest(news.Message);
            }
            
            var resource = _mapper.Map<IEnumerable<New>, IEnumerable<NewResource>>(news.Result);
            return Ok(resource);
        }

        [ModelValidation]
        [HttpPost("search")]
        public async Task<IActionResult> SearchTitle([FromBody] NewsSearchResource value)
        {
            var news = await _newsService.SearchAsync(value.Search);

            if (!news.Success)
            {
                return BadRequest(news.Message);
            }

            var resource = _mapper.Map<IEnumerable<New>, IEnumerable<NewResource>>(news.Result);

            return Ok(resource);
        }
    }
}
