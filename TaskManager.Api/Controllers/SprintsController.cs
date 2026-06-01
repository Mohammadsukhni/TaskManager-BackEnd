using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Constants;
using TaskManager.Core.Dto;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    [Authorize]
    public class SprintsController : BaseController
    {
        private readonly ISprintServices _sprintServices;

        public SprintsController(
            ISprintServices sprintServices,
            ICurrentUserService currentUserService)
            : base(currentUserService)
        {
            _sprintServices = sprintServices;
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<SprintDto>>> GetAllSprints(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var sprints = await _sprintServices.GetAllSprintsAsync(pageNumber, pageSize);

            return Ok(sprints);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet("filter")]
        public async Task<ActionResult<SprintFilterResultDto>> FilterSprints(
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _sprintServices.FilterSprintsAsync(search, pageNumber, pageSize));
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<SprintDto>> GetSprintById(int id)
        {
            var sprint = await _sprintServices.GetSprintByIdAsync(id);

            if (sprint == null)
                return NotFound();

            return Ok(sprint);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateSprint(SprintDto dto)
        {
            await _sprintServices.CreateSprintAsync(dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPut]
        public async Task<IActionResult> UpdateSprint(SprintDto dto)
        {
            await _sprintServices.UpdateSprintAsync(dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSprint(int id)
        {
            await _sprintServices.DeleteSprintAsync(id);

            return Ok();
        }
        [HttpGet("my-sprints")]
        public async Task<ActionResult<PagedResultDto<SprintDto>>> GetMySprints(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var sprints = await _sprintServices.GetUserSprintsAsync(
                CurrentUserId,
                pageNumber,
                pageSize);

            return Ok(sprints);
        }
    }
}
