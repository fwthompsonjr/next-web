using Microsoft.AspNetCore.Mvc;
using next.web.core.extensions;
using next.web.Models;

namespace next.web.Controllers
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Code Analysis",
        "S6967:ModelState.IsValid should be called in controller actions",
        Justification = "Model is not needed for standard http-get pages")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Code Analysis",
        "S6934:A Route attribute should be added to the controller when a route template is specified at the action level",
        Justification = "Controller route is defined in definition of partial class")]
    public partial class HomeController
    {
        [HttpGet("/payment-result")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Code Analysis",
            "S6967:ModelState.IsValid should be called in controller actions",
            Justification = "Model validations are performed against payload.")]
        public async Task<IActionResult> PaymentLanding([FromQuery] string? sts, [FromQuery] string? id)
        {
            const string landing = "payment-result";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var page = await GetLanding(session, landing, sts, id);
            return page;
        }

        [HttpPost("/payment-fetch-intent")]
        public async Task<IActionResult> FetchIntent([FromBody] FetchIntentRequest request)
        {
            const string landing = "payment-fetch-intent";
            var session = HttpContext.Session;
            if (!IsSessionAuthenicated(session)) return Redirect("/home");
            var url = GetRemoteUri(landing, null, null);
            var response = await GetIntent(url, request);
            if (response == null) return Redirect("/error");
            return Json(new { clientSecret = response.ClientSecret });
        }

        private async static Task<FetchIntentResponse?> GetIntent(string url, FetchIntentRequest request)
        {
            using var client = new HttpClient();
            var response = await client.PostAsJsonAsync(url, request);
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return json.ToInstance<FetchIntentResponse>();
        }
    }
}