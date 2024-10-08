using Microsoft.AspNetCore.Mvc;
using next.web.core.interfaces;
using next.web.core.models;

namespace next.web.Controllers
{
    [Route("/app")]
    [ApiController]
    public class AppController(ICountyAuthorizationService service) : ControllerBase
    {
        private readonly ICountyAuthorizationService _authorizationService = service;

        [HttpPost("get-county-code")]
        public IActionResult GetCounty(CountyCodeRequest model)
        {
            if (!ModelState.IsValid) return BadRequest();
            var response = _authorizationService.Models.Find(x => x.Name.Equals(model.Name)) ?? new();
            return Ok(response);
        }

    }
}
