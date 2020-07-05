using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectCode.Application.Service.Exceptions;
using ProjectCode.Domain.Model;
using ProjectCode.Infraestructure.Repository;
using ProjectCode.Tests.Config;
using ProjectCode.Tests.Shared;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ProjectCode.Tests.Fixtures
{
    public class PostProjectControllerTest : ProjectCodeTestConfig
    {
        private const string locationUri = "/api/v2/projects/";

        public PostProjectControllerTest() : base(
            new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("ProjectCodeTestPost")
                .Options)
        {
        }

        [Fact]
        public void PostProject_FullPayload_201_Location()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\":\"EXTERNALID\",\"name\":\"Name\",\"sdlcSystem\": {\"id\": 1}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                var result = controller.createProject().Result;

                var project = GetCreatedObject<ProjectModel>(result);
                
                Assert.True(project.id > 0);
                Assert.True(((CreatedResult)result).StatusCode.Value == (int)HttpStatusCode.Created);
                Assert.True(((CreatedResult)result).Location == locationUri);
            }
        }

        [Fact]
        public void PostProject_MinimalPayload_201_Location()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"EXTERNAL-ID\",\"sdlcSystem\": {\"id\": 1}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                var result = controller.createProject().Result;

                var project = GetCreatedObject<ProjectModel>(result);

                Assert.True(project.id > 0);
                Assert.True(((CreatedResult)result).StatusCode.Value == (int)HttpStatusCode.Created);
                Assert.True(((CreatedResult)result).Location == locationUri);
            }
        }

        [Fact]
        public void PostProject_IllegalValue_400()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"sdlcSystem\": {\"id\": \"Whatever\"}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                Func<Task> action = async () => await controller.createProject();

                action.Should().Throw<BadRequestException>();
            }
        }

        [Fact]
        public void PostProject_MissingExternalId_400()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"sdlcSystem\": {\"id\": 1}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePostProject(It.IsAny<ProjectModel>())).Throws(new BadRequestException(""));

                Func<Task> action = async () => await controller.createProject();

                action.Should().Throw<BadRequestException>();
            }
        }

        [Fact]
        public void PostProject_MissingSystem_400()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"EXTERNAL-ID\"}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePostProject(It.IsAny<ProjectModel>())).Throws(new BadRequestException(""));

                Func<Task> action = async () => await controller.createProject();

                action.Should().Throw<BadRequestException>();
            }
        }

        [Fact]
        public void PostProject_NonExistingSystem_404()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"EXTERNALID\",\"sdlcSystem\": {\"id\": 12345}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePostProject(It.IsAny<ProjectModel>())).Throws(new NotFoundException(""));

                Func<Task> action = async () => await controller.createProject(); 

                action.Should().Throw<NotFoundException>();
            }
        }

        [Fact]
        public void PostProject_ConflictingSystemExternalId_409()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"SAMPLEPROJECT\",\"sdlcSystem\": {\"id\": 1}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePostProject(It.IsAny<ProjectModel>())).Throws(new ConflictException(""));

                Func<Task> action = async () => await controller.createProject(); 

                action.Should().Throw<ConflictException>();
            }
        }

        

        private T GetCreatedObject<T>(IActionResult result)
        {
            var createdObjectResult = (CreatedResult)result;
            return (T)createdObjectResult.Value;
        }
    }
}
