using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectCode.Application.Service.Exceptions;
using ProjectCode.Domain.Model;
using ProjectCode.Infraestructure.Repository;
using ProjectCode.Tests.Config;
using System;
using Xunit;

namespace ProjectCode.Tests.Fixtures
{
    public class ProjectValidatorTest : ProjectCodeTestConfig
    {
        public ProjectValidatorTest() : base(
                new DbContextOptionsBuilder<ApiContext>()
                    .UseInMemoryDatabase("ProjectCodeTestValidator")
                    .Options)
        {
        }

        #region Post Validation
        [Fact]
        public void PostProject_200()
        {
            var body = "{\"externalId\": \"EXTERNALID\",\"name\": \"Name\",\"sdlcSystem\": {\"id\": 1}}";
            var projectModel = JsonConvert.DeserializeObject<ProjectModel>(body);

            var result = projectValidator.ValidatePostProject(projectModel);

            Assert.True(result);
        }

        [Fact]
        public void PostProject_NotContainingExternalId_400()
        {
            var body = "{\"sdlcSystem\": {\"id\": 1}}";
            var projectModel = JsonConvert.DeserializeObject<ProjectModel>(body);

            Func<bool> action = () => projectValidator.ValidatePostProject(projectModel);

            action.Should().Throw<BadRequestException>();
        }

        [Fact]
        public void PostProject_NotContainingSystem_400()
        {
            var body = "{\"externalId\": \"EXTERNAL-ID\"}";
            var projectModel = JsonConvert.DeserializeObject<ProjectModel>(body);

            Func<bool> action = () => projectValidator.ValidatePostProject(projectModel);

            action.Should().Throw<BadRequestException>();
        }

        [Fact]
        public void PostProject_NonExistingSystem_404()
        {
            var body = "{\"externalId\": \"EXTERNALID\",\"sdlcSystem\": {\"id\": 12345}}";
            var projectModel = JsonConvert.DeserializeObject<ProjectModel>(body);

            Func<bool> action = () => projectValidator.ValidatePostProject(projectModel);

            action.Should().Throw<NotFoundException>();
        }

        [Fact]
        public void PostProject_ConflictionSystemExternalId_409()
        {
            var body = " {\"externalId\": \"SAMPLEPROJECT\",\"sdlcSystem\": {\"id\": 1}}";
            var projectModel = JsonConvert.DeserializeObject<ProjectModel>(body);

            Func<bool> action = () => projectValidator.ValidatePostProject(projectModel);

            action.Should().Throw<ConflictException>();
        }
        #endregion

        #region Patch Validation
        [Theory]
        [InlineData(5)]
        public void PatchProject_200(long id)
        {
            using (var _apiContext = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"EXTERNALIDEDITED\",\"name\": \"Name-Edited\",\"sdlcSystem\": {\"id\": 1}}";
                dynamic projectPatchInput = JsonConvert.DeserializeObject(body);

                var projectDTO = _apiContext.Project.FirstOrDefaultAsync(x => x.id == id).Result;

                var result = projectValidator.ValidatePatchProject(id, projectDTO, projectPatchInput);

                Assert.True(result);
            }
        }
        [Theory]
        [InlineData(12345)]
        public void PatchProject_NonExistingProject_404(long id)
        {
            using (var _apiContext = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"EXTERNALIDEDITED\",\"name\": \"Name-Edited\",\"sdlcSystem\": {\"id\": 1}}";
                var action = ExecuteAction(id, _apiContext, body);

                action.Should().Throw<NotFoundException>();
            }
        }

        [Theory]
        [InlineData(1)]
        public void PatchProject_NonExistingSystem_404(long id)
        {
            using (var _apiContext = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"EXTERNALIDEDITED\",\"name\": \"Name-Edited\",\"sdlcSystem\": {\"id\": 12345}}";
                var action = ExecuteAction(id, _apiContext, body);

                action.Should().Throw<NotFoundException>();
            }
        }

        [Theory]
        [InlineData(2)]
        public void PatchProject_ConflictionSystem_409(long id)
        {
            using (var _apiContext = new ApiContext(contextOptions))
            {
                var body = "{\"sdlcSystem\": {\"id\": 2}}";
                var action = ExecuteAction(id, _apiContext, body);

                action.Should().Throw<ConflictException>();
            }
        }

        [Theory]
        [InlineData(1)]
        public void PatchProject_ConflictionExternalId_409(long id)
        {
            using (var _apiContext = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"PROJECTX\"}";
                var action = ExecuteAction(id, _apiContext, body);

                action.Should().Throw<ConflictException>();
            }
        }
        [Theory]
        [InlineData(1)]
        public void PatchProject_ConflictionSystemExternalId_409(long id)
        {
            using (var _apiContext = new ApiContext(contextOptions))
            {
                var body = "{\"externalId\": \"PROJECTX\",\"sdlcSystem\": {\"id\": 2}}";
                var action = ExecuteAction(id, _apiContext, body);

                action.Should().Throw<ConflictException>();
            }
        }

        [Theory]
        [InlineData(1)]
        public void PatchProject_IllegalSystem_400(long id)
        {
            using (var _apiContext = new ApiContext(contextOptions))
            {
                var body = "{\"sdlcSystem\": {\"id\": \"Whatever\"}}";
                var action = ExecuteAction(id, _apiContext, body);

                action.Should().Throw<BadRequestException>();
            }
        }

        private Func<bool> ExecuteAction(long id, ApiContext _apiContext, string body)
        {
            dynamic projectPatchInput = JsonConvert.DeserializeObject(body);

            var projectDTO = _apiContext.Project.FirstOrDefaultAsync(x => x.id == id).Result;

            Func<bool> action = () => projectValidator.ValidatePatchProject(id, projectDTO, projectPatchInput);
            return action;
        }
        #endregion
    }
}
