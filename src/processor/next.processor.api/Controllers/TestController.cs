using Microsoft.AspNetCore.Mvc;
using next.processor.api.interfaces;
namespace next.processor.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IServiceProvider provider) : ControllerBase
    {
        private readonly IServiceProvider _provider = provider;
        [HttpGet("install-browser")]
        public async Task<ActionResult> BrowserInstallAsync([FromQuery] string? os = "linux")
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrEmpty(os)) os = "linux";
            var key = $"{os}-firefox";
            var service = _provider.GetKeyedService<IWebContainerInstall>(key);
            if (service == null) { return BadRequest("Unable to create installation instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted)
            {
                var message = new
                {
                    description = "Failed to install firefox component"
                };
                return BadRequest(message);
            }
            return Ok("firefox has been installed.");
        }

        [HttpGet("install-driver")]
        public async Task<ActionResult> InstallAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("linux-geckodriver");
            if (service == null) { return BadRequest("Unable to create installation instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted)
            {
                var message = new
                {
                    description = "Failed to install geckodriver component"
                };
                return BadRequest(message);
            }
            return Ok("geckodriver has been installed.");
        }

        [HttpGet("verify-driver")]
        public async Task<ActionResult> VerifyAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("verification");
            if (service == null) { return BadRequest("Unable to create verification instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted)
            {
                var message = new
                {
                    description = "Failed to execute verification component"
                };
                return BadRequest(message);
            }
            return Ok("verification is successful.");
        }

        [HttpGet("read-check-collin")]
        public async Task<ActionResult> ReadCollinDataAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("read-collin");
            if (service == null) { return BadRequest("Unable to create test instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted)
            {
                var message = new
                {
                    description = "Failed to execute search"
                };
                return BadRequest(message);
            }
            return Ok("test is successful.");
        }

        [HttpGet("read-check-denton")]
        public async Task<ActionResult> ReadDentonDataAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("read-denton");
            if (service == null) { return BadRequest("Unable to create test instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted)
            {
                var message = new
                {
                    description = "Failed to execute search"
                };
                return BadRequest(message);
            }
            return Ok("test is successful.");
        }

        [HttpGet("read-check-harris")]
        public async Task<ActionResult> ReadHarrisDataAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("read-harris");
            if (service == null) { return BadRequest("Unable to create test instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted)
            {
                var message = new
                {
                    description = "Failed to execute search"
                };
                return BadRequest(message);
            }
            return Ok("test is successful.");
        }

        [HttpGet("read-check-tarrant")]
        public async Task<ActionResult> ReadTarrantDataAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("read-tarrant");
            if (service == null) { return BadRequest("Unable to create test instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted)
            {
                var message = new
                {
                    description = "Failed to execute search"
                };
                return BadRequest(message);
            }
            return Ok("test is successful.");
        }

    }
}
