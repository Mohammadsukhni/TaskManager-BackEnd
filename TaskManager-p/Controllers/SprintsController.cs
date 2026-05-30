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
    public class SprintsController : ControllerBase
    {
        private readonly ISprintServices _sprintServices;

        public SprintsController(ISprintServices sprintServices)
        {
            _sprintServices = sprintServices;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<SprintDto>>> GetAllSprints()
        {
            var sprints = await _sprintServices.GetAllSprintsAsync();

            return Ok(sprints);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<SprintDto>> GetSprintById(int id)
        {
            var sprint = await _sprintServices.GetSprintByIdAsync(id);

            if (sprint == null)
                return NotFound();

            return Ok(sprint);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateSprint(SprintDto dto)
        {
            try
            {
                await _sprintServices.CreateSprintAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateSprint(SprintDto dto)
        {
            try
            {
                await _sprintServices.UpdateSprintAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSprint(int id)
        {
            await _sprintServices.DeleteSprintAsync(id);

            return Ok();
        }
        [HttpGet("my-sprints")]
        public async Task<IActionResult> GetMySprints()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var sprints = await _sprintServices.GetUserSprintsAsync(userId);

            return Ok(sprints);
        }
    }
}
