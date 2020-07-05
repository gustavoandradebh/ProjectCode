using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProjectCode.Application.Service;
using ProjectCode.Domain.Interfaces;
using ProjectCode.Infraestructure.Mapper.Mapper;
using ProjectCode.Infraestructure.Repository;
using ProjectCode.Ui.Api.Controllers;
using System.IO;

namespace ProjectCode.Tests.Shared
{
    public static class Util
    {
        public static HttpContext ConfigRequest(string data)
        {
            var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(data));

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Body = stream;
            httpContext.Request.ContentLength = stream.Length;

            return httpContext;
        }

        public static ProjectsController ConfigController(ApiContext context, string data, MapperProvider mapperProvider, Mock<IProjectValidator> projectValidator)
        {
            var service = new ProjectAppService(context, mapperProvider, projectValidator.Object);

            var controllerContext = new ControllerContext()
            {
                HttpContext = ConfigRequest(data),
            };

            var controller = new ProjectsController(service)
            {
                ControllerContext = controllerContext
            };

            return controller;
        }
    }
}
