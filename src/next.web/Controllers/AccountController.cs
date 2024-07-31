using Microsoft.AspNetCore.Mvc;

namespace next.web.Controllers
{
    [Route("/my-account")]
    public class AccountController : BaseController
    {
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
