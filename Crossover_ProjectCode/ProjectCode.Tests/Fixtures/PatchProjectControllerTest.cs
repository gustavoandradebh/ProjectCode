using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectCode.Application.Service.Exceptions;
using ProjectCode.Domain.DataTransferObject;
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
    public class PatchProjectControllerTest : ProjectCodeTestConfig
    {
        public PatchProjectControllerTest() : base(
                new DbContextOptionsBuilder<ApiContext>()
                    .UseInMemoryDatabase("ProjectCodeTestPatch")
                    .Options)
        {
        }

        [Fact]
        public void PatchProject_FullPayload_200_ChangedFields()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"EXTERNALIDEDITED\",\"name\": \"Name-Edited\",\"sdlcSystem\": {\"id\": 1}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                var result = controller.updateProject(5).Result;

                var project = GetOkObject<ProjectModel>(result);

                Assert.True(((OkObjectResult)result).StatusCode.Value.Equals((int)HttpStatusCode.OK));
                Assert.True(project.externalId.Equals("EXTERNALIDEDITED"));
                Assert.True(project.name.Equals("Name-Edited"));
                Assert.True(project.sdlcSystem.id.GetValueOrDefault().Equals(1));

            }
        }

        [Fact]
        public void PatchProject_PayloadOnlyExternalId_200_SameNameSystem()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"EXTERNAL-IDEDITED\"}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                var result = controller.updateProject(6).Result;

                var project = GetOkObject<ProjectModel>(result);

                Assert.True(((OkObjectResult)result).StatusCode.Value.Equals((int)HttpStatusCode.OK));
                Assert.True(project.externalId.Equals("EXTERNAL-IDEDITED"));
                Assert.True(project.name.Equals("Project One"));
                Assert.True(project.sdlcSystem.id.GetValueOrDefault().Equals(3));

            }
        }

        [Fact]
        public void PatchProject_PayloadOnlySystem_200_SameNameExternalId()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"sdlcSystem\": {\"id\": 1}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                var result = controller.updateProject(7).Result;

                var project = GetOkObject<ProjectModel>(result);

                Assert.True(((OkObjectResult)result).StatusCode.Value.Equals((int)HttpStatusCode.OK));
                Assert.True(project.externalId.Equals("PROJECTTWO"));
                Assert.True(project.name.Equals("Project Two"));
                Assert.True(project.sdlcSystem.id.GetValueOrDefault().Equals(1));

            }
        }

        [Fact]
        public void PatchProject_EmptyPayload_200_NoChangedFields()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                var result = controller.updateProject(8).Result;

                var project = GetOkObject<ProjectModel>(result);

                Assert.True(((OkObjectResult)result).StatusCode.Value.Equals((int)HttpStatusCode.OK));
                Assert.True(project.externalId.Equals("PROJECTTHREE"));
                Assert.True(project.name.Equals("Project Three"));
                Assert.True(project.sdlcSystem.id.GetValueOrDefault().Equals(3));

            }
        }

        [Fact]
        public void PatchProject_PayloadNullName_200_NameNull()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"name\": null}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                var result = controller.updateProject(5).Result;

                var project = GetOkObject<ProjectModel>(result);

                Assert.True(((OkObjectResult)result).StatusCode.Value.Equals((int)HttpStatusCode.OK));
                Assert.True(project.name == null);
            }
        }

        [Fact]
        public void PatchProject_PayloadIllegalValue_400()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"sdlcSystem\": {\"id\": \"Whatever\"}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePatchProject(It.IsAny<long>(), It.IsAny<Project>(),
                    It.IsAny<object>())).Throws(new BadRequestException(""));

                Func<Task> action = async () => await controller.updateProject(1);

                action.Should().Throw<BadRequestException>();
            }
        }

        [Fact]
        public void PatchProject_PayloadNonExistingSystem_404()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"sdlcSystem\": {\"id\": 12345}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePatchProject(It.IsAny<long>(), It.IsAny<Project>(), 
                    It.IsAny<object>())).Throws(new NotFoundException(""));

                Func<Task> action = async () => await controller.updateProject(1);

                action.Should().Throw<NotFoundException>();
            }
        }

        [Fact]
        public void PatchProject_PayloadConflictingSystem_409()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"sdlcSystem\": {\"id\": 2}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePatchProject(It.IsAny<long>(), It.IsAny<Project>(),
                    It.IsAny<object>())).Throws(new ConflictException(""));

                Func<Task> action = async () => await controller.updateProject(1);

                action.Should().Throw<ConflictException>();
            }
        }

        [Fact]
        public void PatchProject_PayloadConflictingExternalId_409()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"PROJECTX\"}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePatchProject(It.IsAny<long>(), It.IsAny<Project>(),
                    It.IsAny<object>())).Throws(new ConflictException(""));

                Func<Task> action = async () => await controller.updateProject(1);

                action.Should().Throw<ConflictException>();
            }
        }

        [Fact]
        public void PatchProject_PayloadConflictingSystemExternalId_409()
        {
            using (var context = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"PROJECTX\",\"sdlcSystem\": {\"id\": 2}}";
                var controller = Util.ConfigController(context, body, mapperProvider, projectValidatorMock);

                projectValidatorMock.Setup(x => x.ValidatePatchProject(It.IsAny<long>(), It.IsAny<Project>(),
                    It.IsAny<object>())).Throws(new ConflictException(""));

                Func<Task> action = async () => await controller.updateProject(1);

                action.Should().Throw<ConflictException>();
            }
        }

        private T GetOkObject<T>(IActionResult result)
        {
            var createdObjectResult = (OkObjectResult)result;
            return (T)createdObjectResult.Value;
        }
    }
}
