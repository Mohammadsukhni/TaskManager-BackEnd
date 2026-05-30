using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Core.Dto;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectServices _projectServices;

        public ProjectsController(IProjectServices projectServices)
        {
            _projectServices = projectServices;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetAllProjects()
        {
            var projects = await _projectServices.GetAllProjectsAsync();

            return Ok(projects);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            var project = await _projectServices.GetProjectByIdAsync(id);

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [HttpGet("my-projects")]
        public async Task<IActionResult> GetMyProjects()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var projects = await _projectServices.GetUserProjectsAsync(userId);

            return Ok(projects);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectDto dto)
        {
            await _projectServices.CreateProjectAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateProject(ProjectDto dto)
        {
            await _projectServices.UpdateProjectAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectServices.DeleteProjectAsync(id);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{projectId}/assign-user/{userId}")]
        public async Task<IActionResult> AssignUserToProject(int projectId, int userId)
        {
            try
            {
                await _projectServices.AssignUserToProjectAsync(projectId, userId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }
    }
}
