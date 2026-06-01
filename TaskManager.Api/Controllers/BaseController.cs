using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected BaseController(ICurrentUserService currentUserService)
        {
            CurrentUserService = currentUserService;
        }

        protected ICurrentUserService CurrentUserService { get; }

        protected int CurrentUserId => CurrentUserService.GetUserId();
    }
}
