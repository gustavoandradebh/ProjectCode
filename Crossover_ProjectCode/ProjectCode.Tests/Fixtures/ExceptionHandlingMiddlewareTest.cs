using FluentAssertions;
using Microsoft.AspNetCore.Http;
using ProjectCode.Application.Service.Exceptions;
using ProjectCode.Infraestructure.Middleware;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace ProjectCode.Tests.Fixtures
{
    public class ExceptionHandlingMiddlewareTest
    {
        [Fact]
        public async Task WhenANormalRequestIsMade()
        {
            // Arrange
            RequestDelegate next = (HttpContext hc) => Task.CompletedTask;
            var middleware = new ExceptionHandlingMiddleware(next);

            var context = new DefaultHttpContext();
            await AssertTest(context, middleware);

            context.Response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.OK);
        }
        [Fact]
        public async Task WhenANotFoundExceptionIsRaised()
        {
            var middleware = new ExceptionHandlingMiddleware((innerHttpContext) =>
            {
                throw new NotFoundException("Not found");
            });

            var context = new DefaultHttpContext();
            var streamText = await AssertTest(context, middleware);

            streamText.Should().BeEquivalentTo("Not found");

            context.Response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task WhenAConflictExceptionIsRaised()
        {
            var middleware = new ExceptionHandlingMiddleware((innerHttpContext) =>
            {
                throw new ConflictException("Conflict exception");
            });

            var context = new DefaultHttpContext();
            var streamText = await AssertTest(context, middleware);

            streamText.Should().BeEquivalentTo("Conflict exception");

            context.Response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task WhenABadRequestExceptionIsRaised()
        {
            var middleware = new ExceptionHandlingMiddleware((innerHttpContext) =>
            {
                throw new BadRequestException("Bad request exception");
            });

            var context = new DefaultHttpContext();
            var streamText = await AssertTest(context, middleware);

            streamText.Should().BeEquivalentTo("Bad request exception");

            context.Response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task WhenAnUnExpectedExceptionIsRaised()
        {
            var middleware = new ExceptionHandlingMiddleware(next: (innerHttpContext) =>
            {
                throw new Exception("Test");
            });

            var context = new DefaultHttpContext();
            var streamText = await AssertTest(context, middleware);

            streamText.Should().BeEquivalentTo("Bad Request");

            context.Response.StatusCode
            .Should()
            .Be((int)HttpStatusCode.BadRequest);
        }

        private async Task<string> AssertTest(DefaultHttpContext context, ExceptionHandlingMiddleware middleware)
        {
            context.Response.Body = new MemoryStream();

            await middleware.Invoke(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(context.Response.Body);
            var streamText = reader.ReadToEnd();

            return streamText;
        }
    }
}
