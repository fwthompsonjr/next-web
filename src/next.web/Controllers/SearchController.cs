using Microsoft.AspNetCore.Mvc;

namespace next.web.Controllers
{
    [Route("/search")]
    public class SearchController : BaseController
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session;
            var content = await GetAuthenicatedPage(session, "myaccount");
            return new ContentResult
            {
                ContentType = "text/html",
                Content = content
            };
        }

        [HttpGet]
        [Route("active")]
        public async Task<IActionResult> Active()
        {
            return await Index();
        }

        [HttpGet]
        [Route("purchases")]
        public async Task<IActionResult> Purchases()
        {
            return await Index();
        }


        [HttpGet]
        [Route("history")]
        public async Task<IActionResult> History()
        {
            return await Index();
        }
    }
}
