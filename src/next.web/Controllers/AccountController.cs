using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/my-account")]
    public class AccountController : BaseController
    {
        [HttpGet]
        [Route("home")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Index()
        {
            return await GetPage("account-home");
        }

        [HttpGet]
        [Route("profile")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Profile()
        {
            return await GetPage("account-profile");
        }

        [HttpGet]
        [Route("permissions")]
        [OutputCache(Duration = 10)]
        public async Task<IActionResult> Permissions()
        {
            return await GetPage("account-permissions");
        }

        private async Task<IActionResult> GetPage(string viewName)
        {
            var session = HttpContext.Session;
            var content = await GetAuthenicatedPage(session, "myaccount");
            var viewer = AppContainer.GetDocumentView(viewName);
            if (viewer == null)
            {
                return new ContentResult
                {
                    ContentType = "text/html",
                    Content = RemoveHeaderDuplicate(content)
                };
            }

            content = viewer.SetMenu(content);
            content = viewer.SetChildMenu(content);
            content = viewer.SetTab(content);

            return new ContentResult
            {
                ContentType = "text/html",
                Content = RemoveHeaderDuplicate(content)
            };
        }
    }
}
