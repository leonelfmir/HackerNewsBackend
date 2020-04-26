using AutoFixture.Xunit2;
using HackerNews.Domain.Models;
using HackerNews.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using FluentAssertions;

namespace HackerNews.Persistence.Tests.Repositories
{
    public class NewsRepositoryTest
    {
        private readonly IMemoryCache _memoryCache;

        public NewsRepositoryTest()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var serviceProvider = services.BuildServiceProvider();
            var memoryCache = serviceProvider.GetService<IMemoryCache>();
            _memoryCache = memoryCache;
        }

        [Theory, AutoData]
        public async Task ListAsync_ShouldGetAllNews(List<New> newsFixture)
        {
            // Arrange
            var newIdsUrl = "http://news.com";
            var itemByIdUrl = "http://newIds/{0}.com";
            var mockedHttpClientFactory = Substitute.For<IHttpClientFactory>();

            var requestsById = createRequestsById(newsFixture, itemByIdUrl);

            var ids = newsFixture.Select(x => x.Id);
            var requests = requestsById.Concat(new List<(string, object)> { (newIdsUrl, ids) });
            var mockedHttpMessageHandler = new MockHttpMessageHandler(requests);

            var fakeHttpClient = new HttpClient(mockedHttpMessageHandler);

            mockedHttpClientFactory.CreateClient().Returns(fakeHttpClient);

            var mockedConfiguration = Substitute.For<IConfiguration>();
            mockedConfiguration["ApiUrls:newIds"].Returns(newIdsUrl);
            mockedConfiguration["ApiUrls:itemById"].Returns(itemByIdUrl);

            var sut = new NewsRepository(mockedConfiguration, mockedHttpClientFactory, _memoryCache);

            // Act
            var result = await sut.ListAsync();

            // Asert
            result.Should().BeEquivalentTo(newsFixture);
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
