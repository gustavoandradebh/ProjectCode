using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Moq;
using ProjectCode.Application.Service.Validators;
using ProjectCode.Domain.DataTransferObject;
using ProjectCode.Domain.Interfaces;
using ProjectCode.Infraestructure.Mapper;
using ProjectCode.Infraestructure.Mapper.Mapper;
using ProjectCode.Infraestructure.Repository;
using System;

namespace ProjectCode.Tests.Config
{
    public class ProjectCodeTestConfig
    {
        private ApiContext _apiContext;

        protected Mock<IProjectValidator> projectValidatorMock;
        protected ProjectValidator projectValidator;
        protected ProjectCodeTestConfig(DbContextOptions<ApiContext> contextOptions)
        {
            this.contextOptions = contextOptions;
            projectValidatorMock = new Mock<IProjectValidator>();
            projectValidator = new ProjectValidator(new ApiContext(contextOptions));
            CreateMapper();

            Seed();
        }

        private void CreateMapper()
        {
            var config = new MapperConfiguration(o =>
            {
                o.AddProfile(new ProjectMapping());
                o.AddProfile(new SdlcSystemMapping());
            });
            mapperProvider = new MapperProvider(config.CreateMapper());
        }

        protected DbContextOptions<ApiContext> contextOptions { get; }

        protected MapperProvider mapperProvider { get; set; }

        #region SeedData
        private void Seed()
        {
            using (_apiContext = new ApiContext(contextOptions))
            {
                _apiContext.Database.EnsureDeleted();
                _apiContext.Database.EnsureCreated();

                CreateSdlcSystem("http://jira.company.com", "Company JIRA");
                CreateSdlcSystem("http://bugzilla.company.com", "Company BugZilla");
                CreateSdlcSystem("http://mantis.company.com", "Company Mantis");

                _apiContext.SaveChanges();

                createProject("SAMPLEPROJECT", "Sample Project", 1);
                createProject("PROJECTX", "Project X", 2);
                createProject("SAMPLEPROJECT", "Sample Project", 2);
                createProject("PROJECTX", "Project X", 1);
                createProject("PROJECTZERO", "Project Zero", 3);
                createProject("PROJECTONE", "Project One", 3);
                createProject("PROJECTTWO", "Project Two", 3);
                createProject("PROJECTTHREE", "Project Three", 3);

                _apiContext.SaveChanges();
            }
        }

        private void createProject(string external_id, string projectName, int sdlc_system_id)
        {
            var project = new Project
            {
                externalId = external_id,
                name = projectName,
                sdlcSystemId = sdlc_system_id,
                createdDate = DateTime.Now,
                lastModifiedDate = DateTime.Now
            };

            _apiContext.Add(project);
        }

        private void CreateSdlcSystem(string base_url, string description)
        {
            var sdlcSystem = new Sdlc_System
            {
                baseUrl = base_url,
                description = description,
                createdDate = DateTime.Now,
                lastModifiedDate = DateTime.Now
            };

            _apiContext.Add(sdlcSystem);
        }
        #endregion
    }
}
