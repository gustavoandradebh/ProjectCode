using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProjectCode.Application.Service;
using ProjectCode.Application.Service.Validators;
using ProjectCode.Domain.DataTransferObject;
using ProjectCode.Domain.Interfaces;
using ProjectCode.Infraestructure.Mapper;
using ProjectCode.Infraestructure.Mapper.Mapper;
using ProjectCode.Infraestructure.Middleware;
using ProjectCode.Infraestructure.Repository;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProjectCode.Ui.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private ApiContext _apiContext { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();

            services.AddDbContext<ApiContext>(opt =>
               opt.UseInMemoryDatabase("ProjectCode"));

            services
                .AddSingleton<IMapperProvider, MapperProvider>()
                
                .AddAutoMapper(new Type[] { typeof(ProjectMapping), typeof(SdlcSystemMapping) })

                .AddScoped<IProjectValidator, ProjectValidator>()
                .AddScoped<IProjectService, ProjectAppService>();

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.IgnoreNullValues = false;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.WriteIndented = true;
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                }); ;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseCors("AllowAll");

            app.UseAuthorization();
            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            

            _apiContext = serviceProvider.GetService<ApiContext>();
            Seed();
        }

        private void Seed()
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
    }
}
