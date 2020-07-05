using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectCode.Application.Service;
using ProjectCode.Application.Service.Exceptions;
using ProjectCode.Domain.Interfaces;
using ProjectCode.Infraestructure.Repository;
using ProjectCode.Tests.Config;
using ProjectCode.Ui.Api.Controllers;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Xunit;

namespace ProjectCode.Tests.Fixtures
{
    public class GetProjectControllerTest: ProjectCodeTestConfig
    {
        private readonly Mock<IProjectValidator> _projectValidator;

        public GetProjectControllerTest() : base(
            new DbContextOptionsBuilder<ApiContext>()
                .UseInMemoryDatabase("ProjectCodeTestGet")
                .Options)
        {
            _projectValidator = new Mock<IProjectValidator>();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void GetProject_Success(long id)
        {
            using(var context = new ApiContext(contextOptions))
            {
                var service = new ProjectAppService(context, mapperProvider, _projectValidator.Object);
                var controller = new ProjectsController(service);

                var result = controller.getProject(id);

                Assert.IsAssignableFrom<OkObjectResult>(result.Result);
            }
        }

        [Theory]
        [InlineData(1123)]
        [InlineData(545)]
        public void GetProject_NotFound(long id)
        {
            using (var context = new ApiContext(contextOptions))
            {
                var service = new ProjectAppService(context, mapperProvider, _projectValidator.Object);
                var controller = new ProjectsController(service);

                Func<Task> action = async () => await controller.getProject(id); 

                action.Should().Throw<NotFoundException>();
            }


        }
    }
}
