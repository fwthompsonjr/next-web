﻿using Microsoft.AspNetCore.Mvc;
using next.core.interfaces;
using next.web.core.extensions;
using next.web.core.util;

namespace next.web.Controllers
{
    [Route("/correspondence")]
    public class MailController(IApiWrapper wrapper, IViolationService violations) : BaseController(wrapper, violations)
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var content = await GetAuthenicatedPage(session, "mailbox");
            var api = AppContainer.ServiceProvider?.GetService<IPermissionApi>();
            if (api != null) content = await session.GetMailBox(api, content, apiwrapper);
            content = await AppendStatus(content, true);
            content = GetHttpRedirect(content, session);
            return GetResult(content);
        }
    }
}