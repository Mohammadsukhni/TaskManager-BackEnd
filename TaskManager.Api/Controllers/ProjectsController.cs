using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Constants;
using TaskManager.Core.Dto;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    [Authorize]
    public class ProjectsController : BaseController
    {
        private readonly IProjectServices _projectServices;

        public ProjectsController(
            IProjectServices projectServices,
            ICurrentUserService currentUserService)
            : base(currentUserService)
        {
            _projectServices = projectServices;
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<ProjectDto>>> GetAllProjects(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var projects = await _projectServices.GetAllProjectsAsync(pageNumber, pageSize);

            return Ok(projects);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet("filter")]
        public async Task<ActionResult<ProjectFilterResultDto>> FilterProjects(
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _projectServices.FilterProjectsAsync(search, pageNumber, pageSize));
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProjectById(int id)
        {
            var project = await _projectServices.GetProjectByIdAsync(id);

            if (project == null)
                return NotFound();

            return Ok(project);
        }

        [HttpGet("my-projects")]
        public async Task<ActionResult<PagedResultDto<ProjectDto>>> GetMyProjects(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var projects = await _projectServices.GetUserProjectsAsync(
                CurrentUserId,
                pageNumber,
                pageSize);

            return Ok(projects);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateProject(ProjectDto dto)
        {
            await _projectServices.CreateProjectAsync(dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPut]
        public async Task<IActionResult> UpdateProject(ProjectDto dto)
        {
            await _projectServices.UpdateProjectAsync(dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await _projectServices.DeleteProjectAsync(id);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPost("{projectId}/assign-user/{userId}")]
        public async Task<IActionResult> AssignUserToProject(int projectId, int userId)
        {
            await _projectServices.AssignUserToProjectAsync(projectId, userId);

            return Ok();
        }
    }
}
