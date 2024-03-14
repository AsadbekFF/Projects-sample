using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectWithAuthenticationSample.Controllers.Base;
using ProjectWithAuthenticationSample.Infrastructure;
using ProjectWithAuthenticationSample.Infrastructure.Authentication;
using ProjectWithAuthenticationSample.ViewModels;
using Sample.BLL.Infrastructure;
using Sample.BLL.Services;
using Sample.Common.Entities;
using Sample.DAL.Infrastructure;

namespace ProjectWithAuthenticationSample.Controllers
{
    public class UserController : CrudController<User, UserViewModel, int, MasterContext>
    {
        private readonly UserService _userService;
        public UserController(Sample.Common.Logging.ILogger logger, IMapper mapper, UserService service) : base(logger, mapper, service)
        {
            _userService = service;
        }

        [HttpGet("log-in")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("log-in")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LogInViewModel model, string? returnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userService.AuthenticateAsync(model.Username, model.Password);
                    await HttpContext.AuthenticateUser(user, model.RememberMe, default);
                }
                catch (Exception ex)
                {
                    Logger.Error("Error while getting security parameters:", ex);
                    var result = HandleException(ex);
                    if (result != null)
                        return result;
                }
            }

            if (!string.IsNullOrWhiteSpace(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
