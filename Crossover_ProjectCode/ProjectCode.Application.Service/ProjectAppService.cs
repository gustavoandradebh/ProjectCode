using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProjectCode.Application.Service.Exceptions;
using ProjectCode.Domain.DataTransferObject;
using ProjectCode.Domain.Interfaces;
using ProjectCode.Domain.Model;
using ProjectCode.Infraestructure.Repository;
using System;
using System.Threading.Tasks;

namespace ProjectCode.Application.Service
{
    public class ProjectAppService : IProjectService
    {
        private readonly ApiContext _apiContext;
        private readonly IProjectValidator _projectValidator;
        private readonly IMapperProvider _mapper;

        public ProjectAppService(ApiContext apiContext, IMapperProvider mapper, IProjectValidator projectValidator)
        {
            _apiContext = apiContext;
            _mapper = mapper;
            _projectValidator = projectValidator;
        }

        public async Task<ProjectModel> Create(string projectRaw)
        {
            ProjectModel project;
            try
            {
                project = JsonConvert.DeserializeObject<ProjectModel>(projectRaw);
            }
            catch
            {
                throw new BadRequestException("Bad request");
            }

            _projectValidator.ValidatePostProject(project);

            var projectDTO = _mapper.Map<ProjectModel, Project>(project);
            projectDTO.createdDate = DateTime.Now;
            projectDTO.lastModifiedDate = DateTime.Now;

            _apiContext.Project.Add(projectDTO);
            await _apiContext.SaveChangesAsync();

            var createdProject = await Get(projectDTO.id);
            return createdProject;

        }

        public async Task<ProjectModel> Get(long id)
        {

            var projectDTO = await _apiContext.Project.AsQueryable().FirstOrDefaultAsync(x => x.id == id);

            if (projectDTO == null)
                throw new NotFoundException("Project not found.");

            var project = _mapper.Map<ProjectModel>(projectDTO);

            var sdlcSystemDTO = await _apiContext.SdlcSystem.FirstOrDefaultAsync(x => x.id == projectDTO.sdlcSystemId);
            project.sdlcSystem = _mapper.Map<SdlcSystemModel>(sdlcSystemDTO);
            return project;

        }

        public async Task<ProjectModel> Update(long id, string projectRaw)
        {
            dynamic projectPatchInput = JsonConvert.DeserializeObject(projectRaw);

            var projectDTO = await _apiContext.Project.FirstOrDefaultAsync(x => x.id == id);

            _projectValidator.ValidatePatchProject(id, projectDTO, projectPatchInput);

            var fieldsUpdated = false;
            fieldsUpdated = UpdateExternalId(projectPatchInput, projectDTO);
            fieldsUpdated = UpdateName(projectRaw, projectPatchInput, projectDTO);
            fieldsUpdated = UpdateSdlcSystem(projectPatchInput, projectDTO);

            if (fieldsUpdated)
            {
                projectDTO.lastModifiedDate = DateTime.Now;
                _apiContext.SaveChanges();
            }

            return await Get(id);
        }

        private static bool UpdateExternalId(dynamic projectPatchInput, Project projectDTO)
        {
            if (projectPatchInput.externalId != null)
            {
                projectDTO.externalId = projectPatchInput.externalId;
                return true;
            }

            return false;
        }

        private static bool UpdateName(string project, dynamic projectPatchInput, Project projectDTO)
        {
            if (project.Contains("name") && project.Contains("null"))
            {
                projectDTO.name = null;
                return true;
            }
            else
            {
                if (projectPatchInput.name != null)
                {
                    projectDTO.name = projectPatchInput.name;
                    return true;
                }
            }
            return false;
        }

        private static bool UpdateSdlcSystem(dynamic projectPatchInput, Project projectDTO)
        {
            if (projectPatchInput.sdlcSystem != null)
            {
                projectDTO.sdlcSystemId = projectPatchInput.sdlcSystem.id;
                return true;
            }

            return false;
        }
    }
}
