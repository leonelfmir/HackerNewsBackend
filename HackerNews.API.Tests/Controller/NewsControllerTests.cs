using AutoMapper;
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
        private Mock<INewsService> _mockNewsService;

        private New _newFixture;

        private IEnumerable<New> _newFixtures;

        private GenericResponse<IEnumerable<New>> _responseFixture;

        public NewsControllerTests()
        {
            _newFixture = new New
            {
                By = "a",
                Id = 1,
                Time = DateTime.Now,
                Title = "t",
                Type = "ty",
                Url = "url"
            };

            _newFixtures = new List<New> { _newFixture };

            _responseFixture = new GenericResponse<IEnumerable<New>>(_newFixtures);

            _mockNewsService = new Mock<INewsService>();

            _mockNewsService.Setup(svc => svc.ListAsync()).ReturnsAsync(_responseFixture);

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ModelToResourceProfile());
            });

            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task Get_ShouldCallNewsServiceOnce()
        {
            // Arrange
            var controller = new NewsController(_mapper, _mockNewsService.Object);

            // Act
            var result = await controller.Get();

            // Asset
            _mockNewsService.Verify(svc => svc.ListAsync(), Times.Once());
        }
        
        [Fact]
        public async Task Get_TitlesShouldBeTheSame()
        {
            // Arrange
            var controller = new NewsController(_mapper, _mockNewsService.Object);

            // Act
            var result = await controller.Get();

            // Asset
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnNews = Assert.IsAssignableFrom<IEnumerable<NewResource>>(okResult.Value);
            var firstValue = returnNews.FirstOrDefault();
            Assert.Equal(firstValue?.Title, _newFixture.Title);
        }
        
        [Fact]
        public async Task Get_ByShouldBeAuthor()
        {
            // Arrange
            var controller = new NewsController(_mapper, _mockNewsService.Object);

            // Act
            var result = await controller.Get();

            // Asset
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnNews = Assert.IsAssignableFrom<IEnumerable<NewResource>>(okResult.Value);
            var firstValue = returnNews.FirstOrDefault();
            Assert.Equal(firstValue?.Author, _newFixture.By);
        }
    }
}
