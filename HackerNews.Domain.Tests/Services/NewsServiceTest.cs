using AutoFixture.Xunit2;
using FluentAssertions;
using HackerNews.Domain.Communication;
using HackerNews.Domain.Models;
using HackerNews.Domain.Repositories;
using HackerNews.Domain.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HackerNews.Domain.Tests.Services
{
    public class NewsServiceTest
    {
        [Theory, AutoData]
        public async Task ListAsync_ShouldCallRepositoryOnce(List<New> newsFixture)
        {
            // Arrange
            var mockedRepoService = createNewsRepository(newsFixture);
            var sut = new NewsService(mockedRepoService.Object);

            // Act
            var result = await sut.ListAsync();

            // Asert
            mockedRepoService.Verify(svc => svc.ListAsync(), Times.Once());

        }

        [Theory, AutoData]
        public async Task ListAsync_ShouldReturnRightType(List<New> newsFixture)
        {
            // Arrange
            var mockedRepoService = createNewsRepository(newsFixture);
            var sut = new NewsService(mockedRepoService.Object);

            // Act
            var result = await sut.ListAsync();

            // Asert
            Assert.IsType<GenericResponse<IEnumerable<New>>>(result);
        }

        [Fact]
        public async Task ListAsync_ShouldReturnErrorResponseOnError()
        {
            // Arrange
            const string errorMessage = "Error loading news";
            var mockNewsService = new Mock<INewsRepository>();
            mockNewsService.Setup(svc => svc.ListAsync()).Throws(new Exception());
            var sut = new NewsService(mockNewsService.Object);

            // Act
            var result = await sut.ListAsync();

            // Asert
            var res = Assert.IsType<GenericResponse<IEnumerable<New>>>(result);
            res.Success.Should().Be(false);
            res.Message.Should().Be(errorMessage);
        }

        [Theory, AutoData]
        public async Task ListAsync_ShouldReturnCorrectSuccessResponse(List<New> newsFixture)
        {
            // Arrange
            var mockedRepoService = createNewsRepository(newsFixture);
            var sut = new NewsService(mockedRepoService.Object);
            

            // Act
            var result = await sut.ListAsync();

            // Asert
            var res = Assert.IsType<GenericResponse<IEnumerable<New>>>(result);
            res.Success.Should().Be(true);
            res.Result.Should().Equal(newsFixture);
        }

        [Theory, AutoData]
        public async Task SearchAsync_ShouldCallRepositoryOnce(List<New> newsFixture, string searchValue)
        {
            // Arrange
            var mockedRepoService = createNewsRepositoryForSearch(newsFixture);
            var sut = new NewsService(mockedRepoService.Object);

            // Act
            var result = await sut.SearchAsync(searchValue);

            // Asert
            mockedRepoService.Verify(svc => svc.SearchByTitleAsync(searchValue), Times.Once());

        }

        [Theory, AutoData]
        public async Task SearchAsync_ShouldReturnRightType(List<New> newsFixture, string searchValue)
        {
            // Arrange
            var mockedRepoService = createNewsRepositoryForSearch(newsFixture);
            var sut = new NewsService(mockedRepoService.Object);

            // Act
            var result = await sut.SearchAsync(searchValue);

            // Asert
            Assert.IsType<GenericResponse<IEnumerable<New>>>(result);
        }

        [Fact]
        public async Task SearchAsync_ShouldReturnErrorResponseOnError()
        {
            // Arrange
            const string errorMessage = "Error loading news";
            var mockNewsService = new Mock<INewsRepository>();
            mockNewsService.Setup(svc => svc.SearchByTitleAsync(It.IsAny<string>())).Throws(new Exception());
            var sut = new NewsService(mockNewsService.Object);

            // Act
            var result = await sut.SearchAsync("any");

            // Asert
            var res = Assert.IsType<GenericResponse<IEnumerable<New>>>(result);
            res.Success.Should().Be(false);
            res.Message.Should().Be(errorMessage);
        }

        [Theory, AutoData]
        public async Task SearchAsync_ShouldReturnCorrectSuccessResponse(List<New> newsFixture, string searchValue)
        {
            // Arrange
            var mockedRepoService = createNewsRepositoryForSearch(newsFixture);
            var sut = new NewsService(mockedRepoService.Object);
            

            // Act
            var result = await sut.SearchAsync(searchValue);

            // Asert
            var res = Assert.IsType<GenericResponse<IEnumerable<New>>>(result);
            res.Success.Should().Be(true);
            res.Result.Should().Equal(newsFixture);
        }

        private Mock<INewsRepository> createNewsRepository(List<New> newsFixture)
        {
            var mockNewsService = new Mock<INewsRepository>();
            mockNewsService.Setup(svc => svc.ListAsync()).ReturnsAsync(newsFixture);
            return mockNewsService;
        }
        
        private Mock<INewsRepository> createNewsRepositoryForSearch(List<New> newsFixture)
        {
            var mockNewsService = new Mock<INewsRepository>();
            mockNewsService.Setup(svc => svc.SearchByTitleAsync(It.IsAny<string>())).ReturnsAsync(newsFixture);
            return mockNewsService;
        }
    }
}
