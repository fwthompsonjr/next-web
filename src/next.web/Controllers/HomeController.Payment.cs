﻿using Microsoft.AspNetCore.Mvc;

namespace next.web.Controllers
{
    public partial class HomeController
    {
        [HttpGet("/payment-result")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Major Code Smell",
            "S6967:ModelState.IsValid should be called in controller actions",
            Justification = "Model is not needed for standard http-get pages")]
        public async Task<IActionResult> PaymentLanding([FromQuery] string? sts, [FromQuery] string? id)
        {
            const string landing = "payment-result";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var page = await GetLanding(session, landing, sts, id);
            return page;
        }
    }
}