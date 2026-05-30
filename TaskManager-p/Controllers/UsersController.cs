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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAllUsers()
        {
            return Ok(await _userService.GetAllUsersAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser(UserDto dto)
        {
            await _userService.CreateUserAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> UpdateUser(UserDto dto)
        {
            await _userService.UpdateUserAsync(dto);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("change-status")]
        public async Task<IActionResult> ChangeUserStatus(int userId, bool isActive)
        {
            try
            {
                await _userService.ChangeUserStatusAsync(userId, isActive);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [Authorize]
        [HttpGet("my-account")]
        public async Task<ActionResult<UserDto>> GetMyAccount()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [Authorize]
        [HttpPut("my-account")]
        public async Task<IActionResult> UpdateMyAccount(UserDto dto)
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            await _userService.UpdateOwnAccountAsync(userId, dto);

            return Ok();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await _userService.DeleteUserAsync(id);

            return Ok();
        }
    }
}
