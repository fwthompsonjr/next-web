using Microsoft.AspNetCore.Mvc;
using next.web.core.interfaces;
using next.web.core.services;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/my-account")]
    public class AccountController : BaseController
    {
        private readonly AuthorizedUserService? _userService;
        public AccountController(IHttpContextAccessor accessor)
        {
            _userService = new AuthorizedUserService(accessor);
        }

        [HttpGet]
        [Route("home")]
        public IActionResult Index()
        {
            var session = HttpContext.Session;
            var content = GetAuthenicatedPage(session, "myaccount");
            return new ContentResult
            {
                ContentType = "text/html",
                Content = content
            };
        }
    }
}
