using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Constants;
using TaskManager.Core.Dto;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService,
            ICurrentUserService currentUserService)
            : base(currentUserService)
        {
            _userService = userService;
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet]
        public async Task<ActionResult<PagedResultDto<UserDto>>> GetAllUsers(
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _userService.GetAllUsersAsync(pageNumber, pageSize));
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet("filter")]
        public async Task<ActionResult<UserFilterResultDto>> FilterUsers(
            string? search,
            int pageNumber = 1,
            int pageSize = 10)
        {
            return Ok(await _userService.FilterUsersAsync(search, pageNumber, pageSize));
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDto dto)
        {
            await _userService.CreateUserAsync(dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserDto dto)
        {
            await _userService.UpdateUserAsync(dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpPut("change-status")]
        public async Task<IActionResult> ChangeUserStatus(int userId, bool isActive)
        {
            await _userService.ChangeUserStatusAsync(userId, isActive);

            return Ok();
        }

        [Authorize]
        [HttpGet("my-account")]
        public async Task<ActionResult<UserDto>> GetMyAccount()
        {
            var user = await _userService.GetUserByIdAsync(CurrentUserId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpPut("my-account")]
        public async Task<IActionResult> UpdateMyAccount(UserDto dto)
        {
            await _userService.UpdateOwnAccountAsync(CurrentUserId, dto);

            return Ok();
        }

        [Authorize(Roles = ApplicationRoles.Admin)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);

            return Ok();
        }
    }
}
