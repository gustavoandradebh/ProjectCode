using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ProjectCode.Domain.DataTransferObject;
using ProjectCode.Infraestructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ProjectCode.Tests.Fixtures
{
    public class ModelCreationTest
    {
        [Fact]
        public void OnModelCreatingTest()
        {
            DbContextOptionsBuilder<ApiContext> builder = 
                new DbContextOptionsBuilder<ApiContext>().UseInMemoryDatabase(Guid.NewGuid().ToString());

            using (var context = new ApiContext(builder.Options))
            {
                var systemEntity = context.SdlcSystem.FirstOrDefault();
                var project = context.Project.FirstOrDefault();

                systemEntity.Should().BeNull();
                project.Should().BeNull();
            }
        }
    }
}
