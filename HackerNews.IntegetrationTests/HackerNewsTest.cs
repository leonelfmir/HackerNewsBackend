using AutoFixture.Xunit2;
using HackerNews.API;
using HackerNews.Domain.Communication;
using HackerNews.Domain.Models;
using HackerNews.Domain.Services;
using HackerNews.Persistence.Tests;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HackerNews.IntegetrationTests
{
    public class HackerNewsTest : IClassFixture<WebApplicationFactory<HackerNews.API.Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;



        public HackerNewsTest(WebApplicationFactory<API.Startup> factory)
        {
            _factory = factory;
        }

        [Theory, AutoData]
        public async Task GetAllNews(List<New> newsFixture)
        {

            // Arrange
            var url = "/api/news";

            var itemByIdUrl = "https://hacker-news.firebaseio.com/v0/item/{0}.json";
            var newIdsUrl = "https://hacker-news.firebaseio.com/v0/newstories.json";
            
            var requestsById = createRequestsById(newsFixture, itemByIdUrl);

            var ids = newsFixture.Select(x => x.Id);
            var requests = requestsById.Concat(new List<(string, object)> { (newIdsUrl, ids) });
            var mockedHttpMessageHandler = new MockHttpMessageHandler(requests);

            var fakeHttpClient = new HttpClient(mockedHttpMessageHandler);

            var mockedHttpClientFactory = Substitute.For<IHttpClientFactory>();
            mockedHttpClientFactory.CreateClient().Returns(fakeHttpClient);

            var client = _factory.WithWebHostBuilder(builder =>
            {
                var projectDir = Directory.GetCurrentDirectory();
                var configPath = Path.Combine(projectDir, "appsettings.json");

                builder.ConfigureAppConfiguration((context, conf) =>
                {
                    conf.AddJsonFile(configPath);
                });

                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(prov => mockedHttpClientFactory);
                });
            }).CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        private IEnumerable<(string, object)> createRequestsById(List<New> news, string url)
        {
            var res = news.Select(n =>
            {
                var uri = string.Format(url, n.Id);
                return (uri, n as object);
            });

            return res;
        }
    }
}
