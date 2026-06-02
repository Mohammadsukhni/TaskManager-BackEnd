using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Constants;
using TaskManager.Core.Dto;
using TaskManager.Core.Enum;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    [Authorize]
    public class WorkItemsController : BaseController
    {
        private readonly IWorkItemServices _workItemServices;

        public WorkItemsController(
            IWorkItemServices workItemServices,
            ICurrentUserService currentUserService)
            : base(currentUserService)
        {
            _workItemServices = workItemServices;
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<WorkItemDto>>> GetAllWorkItems(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var workItems = await _workItemServices.GetAllWorkItemsAsync(pageNumber, pageSize);

            return Ok(workItems);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet("filter")]
        public async Task<ActionResult<PagedResultDto<WorkItemDto>>> FilterWorkItems(
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _workItemServices.FilterWorkItemsAsync(search, pageNumber, pageSize));
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<WorkItemDto>> GetWorkItemById(int id)
        {
            var workItem = await _workItemServices.GetWorkItemByIdAsync(id);

            if (workItem == null)
                return NotFound();

            return Ok(workItem);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateWorkItem(WorkItemDto dto)
        {
            await _workItemServices.CreateWorkItemAsync(dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPut]
        public async Task<IActionResult> UpdateWorkItem(WorkItemDto dto)
        {
            await _workItemServices.UpdateWorkItemAsync(dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkItem(int id)
        {
            await _workItemServices.DeleteWorkItemAsync(id);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPost("{workItemId}/assign-user/{userId}")]
        public async Task<IActionResult> AssignWorkItemToUser(int workItemId, int userId)
        {
            await _workItemServices.AssignWorkItemToUserAsync(workItemId, userId);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPost("relation")]
        public async Task<IActionResult> AddRelation(int parentWorkItemId, int childWorkItemId)
        {
            await _workItemServices.AddWorkItemRelationAsync(parentWorkItemId, childWorkItemId);
            return Ok();
        }

        [HttpPut("{workItemId}/my-status")]
        public async Task<IActionResult> UpdateMyWorkItemStatus(int workItemId, Status status)
        {
            await _workItemServices.UpdateAssignedWorkItemStatusAsync(CurrentUserId, workItemId, status);

            return Ok();
        }

        [HttpPut("my-workitem")]
        public async Task<IActionResult> UpdateMyWorkItem(WorkItemDto dto)
        {
            await _workItemServices.UpdateAssignedWorkItemAsync(CurrentUserId, dto);

            return Ok();
        }

        [HttpGet("my-workitems")]
        public async Task<ActionResult<PagedResultDto<WorkItemDto>>> GetMyWorkItems(
            int pageNumber = 1,
            int pageSize = 10)
        {
            var workItems =
                await _workItemServices.GetUserWorkItemsAsync(CurrentUserId, pageNumber, pageSize);

            return Ok(workItems);
        }
    }
}
