﻿using ICSharpCode.SharpZipLib.BZip2;
using Microsoft.AspNetCore.Mvc;
using next.processor.api.interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO.Compression;
namespace next.processor.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController(IServiceProvider provider) : ControllerBase
    {
        private readonly IServiceProvider _provider = provider;
        [HttpGet("install-browser")]
        public async Task<ActionResult> BrowserInstallAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("firefox");
            if (service == null) { return BadRequest("Unable to create installation instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted) { return BadRequest("Failed to install firefox component"); }
            return Ok("firefox has been installed.");
        }

        [HttpGet("install-driver")]
        public async Task<ActionResult> InstallAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("geckodriver");
            if (service == null) { return BadRequest("Unable to create installation instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted) { return BadRequest("Failed to install geckodriver component"); }
            return Ok("geckodriver has been installed.");
        }

        [HttpGet("verify-driver")]
        public async Task<ActionResult> VerifyAsync()
        {
            var service = _provider.GetKeyedService<IWebContainerInstall>("verification");
            if (service == null) { return BadRequest("Unable to create verification instance"); }
            var extracted = await service.InstallAsync();
            if (!extracted) { return BadRequest("Failed to execute verification component"); }
            return Ok("verification is successful.");
        }

    }
}
