﻿using AutoFixture.Xunit2;
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

        [Fact]
        public async Task Get_ShouldReturnBadRequestWithMessageIfErroOnService()
        {
            // Arrange
            const string errorMessage = "Error";
            var mockNewsService = new Mock<INewsService>();
            var responseFixture = new GenericResponse<IEnumerable<New>>(errorMessage);
            mockNewsService.Setup(svc => svc.ListAsync()).ReturnsAsync(responseFixture);
            var controller = new NewsController(_mapper, mockNewsService.Object);

            // Act
            var result = await controller.Get();

            // Asset
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().Be(errorMessage);
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

        [Theory, AutoData]
        public async Task Search_ShouldCallNewsServiceOnce(List<New> newsFixture, NewsSearchResource searchValue)
        {
            // Arrange
            var mockedNewsService = createNewsServiceSearch(newsFixture);
            var controller = new NewsController(_mapper, mockedNewsService.Object);

            // Act
            var result = await controller.SearchTitle(searchValue);

            // Asset
            mockedNewsService.Verify(svc => svc.SearchAsync(searchValue.Search), Times.Once());
        }

        [Theory, AutoData]
        public async Task Search_ShouldReturnBadRequestWithMessageIfErroOnService(NewsSearchResource searchValue)
        {
            // Arrange
            const string errorMessage = "Error";
            var mockNewsService = new Mock<INewsService>();
            var responseFixture = new GenericResponse<IEnumerable<New>>(errorMessage);
            mockNewsService.Setup(svc => svc.SearchAsync(searchValue.Search)).ReturnsAsync(responseFixture);
            var controller = new NewsController(_mapper, mockNewsService.Object);

            // Act
            var result = await controller.SearchTitle(searchValue);

            // Asset
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            badRequestResult.Value.Should().Be(errorMessage);
        }

        [Theory, AutoData]
        public async Task Search_FieldsShouldBeTheSame(List<New> newsFixture, NewsSearchResource searchValue)
        {
            // Arrange
            var mockedNewsService = createNewsServiceSearch(newsFixture);
            var controller = new NewsController(_mapper, mockedNewsService.Object);

            // Act
            var result = await controller.SearchTitle(searchValue);

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
        
        private Mock<INewsService> createNewsServiceSearch(List<New> newsFixture)
        {
            var responseFixture = new GenericResponse<IEnumerable<New>>(newsFixture);

            var mockNewsService = new Mock<INewsService>();
            mockNewsService.Setup(svc => svc.SearchAsync(It.IsAny<string>())).ReturnsAsync(responseFixture);
            return mockNewsService;
        }
    }
}
