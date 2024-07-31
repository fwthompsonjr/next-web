﻿using Microsoft.AspNetCore.Mvc;

namespace next.web.Controllers
{
    [Route("/my-account")]
    public class AccountController : BaseController
    {
        [HttpGet]
        [Route("home")]
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
        [Route("profile")]
        public async Task<IActionResult> Profile()
        {
            return await Index();
        }

        [HttpGet]
        [Route("permissions")]
        public async Task<IActionResult> Permissions()
        {
            return await Index();
        }
    }
}
