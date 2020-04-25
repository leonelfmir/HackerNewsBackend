using AutoFixture.Xunit2;
using AutoMapper;
using FluentAssertions;
using HackerNews.API.Controllers;
using HackerNews.API.Mapping;
using HackerNews.API.Resources;
using HackerNews.Domain.Communication;
using HackerNews.Domain.Models;
using HackerNews.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace HackerNews.API.Tests.Controller
{
    public class NewsControllerTests
    {
        private IMapper _mapper;

        public NewsControllerTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ModelToResourceProfile());
            });

            _mapper = config.CreateMapper();
        }

        [Theory, AutoData]
        public async Task Get_ShouldCallNewsServiceOnce(List<New> newsFixture)
        {
            // Arrange
            var mockedNewsService = createNewsService(newsFixture);
            var controller = new NewsController(_mapper, mockedNewsService.Object);

            // Act
            var result = await controller.Get();

            // Asset
            mockedNewsService.Verify(svc => svc.ListAsync(), Times.Once());
        }

        [Theory, AutoData]
        public async Task Get_FieldsShouldBeTheSame(List<New> newsFixture)
        {
            // Arrange
            var mockedNewsService = createNewsService(newsFixture);
            var controller = new NewsController(_mapper, mockedNewsService.Object);

            // Act
            var result = await controller.Get();

            // Asset
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnNews = Assert.IsAssignableFrom<IEnumerable<NewResource>>(okResult.Value);
            returnNews.Should().Equal(newsFixture, (rsc, fix) =>
            {
                return rsc.Author == fix.By && rsc.Title == fix.Title && rsc.Url == fix.Url;
            });
            
        }

        private Mock<INewsService> createNewsService(List<New> newsFixture)
        {
            var responseFixture = new GenericResponse<IEnumerable<New>>(newsFixture);

            var mockNewsService = new Mock<INewsService>();
            mockNewsService.Setup(svc => svc.ListAsync()).ReturnsAsync(responseFixture);
            return mockNewsService;
        }
    }
}
