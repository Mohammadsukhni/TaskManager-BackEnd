using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManager.Core.Dto;
using TaskManager.Core.Enum;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkItemServices _workItemServices;

        public WorkItemsController(IWorkItemServices workItemServices)
        {
            _workItemServices = workItemServices;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<WorkItemDto>>> GetAllWorkItems()
        {
            var workItems = await _workItemServices.GetAllWorkItemsAsync();

            return Ok(workItems);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkItemDto>> GetWorkItemById(int id)
        {
            var workItem = await _workItemServices.GetWorkItemByIdAsync(id);

            if (workItem == null)
                return NotFound();

            return Ok(workItem);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateWorkItem(WorkItemDto dto)
        {
            try
            {
                await _workItemServices.CreateWorkItemAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateWorkItem(WorkItemDto dto)
        {
            try
            {
                await _workItemServices.UpdateWorkItemAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkItem(int id)
        {
            await _workItemServices.DeleteWorkItemAsync(id);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{workItemId}/assign-user/{userId}")]
        public async Task<IActionResult> AssignWorkItemToUser(int workItemId, int userId)
        {
            try
            {
                await _workItemServices.AssignWorkItemToUserAsync(workItemId, userId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("relation")]
        public async Task<IActionResult> AddRelation(int parentWorkItemId, int childWorkItemId)
        {
            await _workItemServices.AddWorkItemRelationAsync(parentWorkItemId, childWorkItemId);
            return Ok();
        }

        [HttpPut("{workItemId}/my-status")]
        public async Task<IActionResult> UpdateMyWorkItemStatus(int workItemId, Status status)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _workItemServices.UpdateAssignedWorkItemStatusAsync(userId, workItemId, status);

            return Ok();
        }

        [HttpPut("my-workitem")]
        public async Task<IActionResult> UpdateMyWorkItem(WorkItemDto dto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _workItemServices.UpdateAssignedWorkItemAsync(userId, dto);

            return Ok();
        }

        [HttpGet("my-workitems")]
        public async Task<IActionResult> GetMyWorkItems()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var workItems =
                await _workItemServices.GetUserWorkItemsAsync(userId);

            return Ok(workItems);
        }
    }
}
