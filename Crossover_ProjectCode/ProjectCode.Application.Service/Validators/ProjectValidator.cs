using ProjectCode.Application.Service.Exceptions;
using ProjectCode.Domain.DataTransferObject;
using ProjectCode.Domain.Interfaces;
using ProjectCode.Domain.Model;
using ProjectCode.Infraestructure.Repository;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace ProjectCode.Application.Service.Validators
{
    public class ProjectValidator : IProjectValidator
    {
        private readonly ApiContext _apiContext;
        public ProjectValidator(ApiContext apiContext)
        {
            _apiContext = apiContext;
        }

        public bool ValidatePostProject(ProjectModel project)
        {
            if (string.IsNullOrEmpty(project.externalId) || project.sdlcSystem == null)
                throw new BadRequestException("Bad Request");

            if (_apiContext.SdlcSystem.FirstOrDefault(queryBySystem(project)) == null)
                throw new NotFoundException("System not found");

            if (_apiContext.Project.Any(queryBySystemAndExternalId(project)))
                throw new ConflictException("Conflict exception");

            return true;
        }

        private static Expression<Func<Sdlc_System, bool>> queryBySystem(ProjectModel project)
        {
            return x => x.id == project.sdlcSystem.id;
        }

        private static Expression<Func<Project, bool>> queryBySystemAndExternalId(ProjectModel project)
        {
            return x => x.sdlcSystemId == project.sdlcSystem.id && x.externalId == project.externalId;
        }

        public bool ValidatePatchProject(long projectId, Project projectDTO, dynamic projectPatchInput)
        {
            if (projectDTO == null)
                throw new NotFoundException("Project not found");

            string externalId = projectPatchInput.externalId;
            if (_apiContext.Project.Any(x => x.externalId == externalId && x.id != projectId))
                throw new ConflictException("Conflict exception");

            if (projectPatchInput.sdlcSystem != null)
            {
                if (!Int64.TryParse(projectPatchInput.sdlcSystem.id.Value.ToString(), out long sdlcSystemId))
                    throw new BadRequestException("Illegal Value");

                if (_apiContext.SdlcSystem.FirstOrDefault(x => x.id == sdlcSystemId) == null)
                    throw new NotFoundException("System not found");

                if (_apiContext.Project.Any(x => x.sdlcSystemId == sdlcSystemId && x.id == projectId))
                    throw new ConflictException("Conflict exception");

            }
            return true;
        }
    }
}
