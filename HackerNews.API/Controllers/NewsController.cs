using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackerNews.API.Filters;
using HackerNews.API.Resources;
using HackerNews.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace HackerNews.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {

            return Ok(new string[] { "value1", "value2" });
        }

        [ModelValidation]
        [HttpPost("search")]
        public async Task<IActionResult> SearchTitle([FromBody] NewsSearchResource value)
        {
            return Ok(value.Search);
        }


        
    }
}
