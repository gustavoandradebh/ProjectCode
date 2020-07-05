using Microsoft.AspNetCore.Mvc;
using ProjectCode.Domain.Interfaces;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCode.Ui.Api.Controllers
{
    [Route("api/v2/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private const string locationUri = "/api/v2/projects/";

        private readonly IProjectService _projectService;
        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet("{id}", Name = "Get a Project")]
        [Route("{id}")]
        public async Task<ActionResult> getProject(long id)
        {
            var project = await _projectService.Get(id);
            return Ok(project);
        }

        [HttpPost(Name = "Create a Project")]
        public async Task<ActionResult> createProject()
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var project = await reader.ReadToEndAsync();
                var projectCreated = await _projectService.Create(project);
                return Created(locationUri, projectCreated);
            }
        }

        [HttpPatch("{id}", Name = "Update a Project")]
        [Route("{id}")]
        public async Task<ActionResult> updateProject(long id)
        {
            using (StreamReader reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                var project = await reader.ReadToEndAsync();
                var projectUpdated = await _projectService.Update(id, project);
                return Ok(projectUpdated);
            }
        }

    }
}
