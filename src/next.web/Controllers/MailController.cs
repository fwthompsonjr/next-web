using Microsoft.AspNetCore.Mvc;

namespace next.web.Controllers
{
    [Route("/correspondence")]
    public class MailController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session;
            var content = await GetAuthenicatedPage(session, "mailbox");
            return new ContentResult
            {
                ContentType = "text/html",
                Content = content
            };
        }
    }
}