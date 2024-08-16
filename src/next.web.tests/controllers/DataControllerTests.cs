using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using next.web.Controllers;
using next.web.core.extensions;
using next.web.core.models;
using next.web.Models;

namespace next.web.tests.controllers
{
    public class DataControllerTests : ControllerTestBase
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ControllerCanBeConstructed(bool authorized)
        {
            var sut = GetProvider(authorized).GetRequiredService<DataController>();
            Assert.NotNull(sut);
        }
        [Theory]
        [InlineData("session-check", false)]
        [InlineData("session-check", true, 0)]
        [InlineData("session-check", true, 1)]
        [InlineData("session-check", false, 1)]
        [InlineData("session-check", true, 2)]
        [InlineData("session-check", false, 2)]
        [InlineData("session-check", true, 3)]
        [InlineData("session-check", false, 3)]
        [InlineData("session-check", true, 4)]
        [InlineData("session-check", false, 4)]
        [InlineData("session-check", false, 5)]
        [InlineData("submit", true, 100)]
        [InlineData("submit", false, 100)]
        [InlineData("submit", false, 101)]
        [InlineData("submit", false, 102)]
        [InlineData("submit", false, 103)]
        [InlineData("submit", false, 104)]
        [InlineData("submit", false, 150)]
        [InlineData("submit", false, 151)]
        [InlineData("submit", false, 152)]
        [InlineData("submit", false, 153)]
        [InlineData("submit", false, 154)]
        [InlineData("submit", false, 155)]
        [InlineData("fetch", true, 200)]
        [InlineData("fetch", false, 200)]
        [InlineData("filter-status", true, 300)]
        [InlineData("filter-status", false, 300)]
        [InlineData("download-verify", true, 400)]
        [InlineData("download-verify", false, 400)]
        [InlineData("download-file-status", false, 500)]
        [InlineData("download-file-status", true, 500)]
        [InlineData("reset-cache", true, 600)]
        [InlineData("reset-cache", false, 600)]
        [InlineData("reset-cache", true, 601)]
        [InlineData("reset-cache", true, 602)]
        [InlineData("reset-cache", true, 603)]
        public async Task ControllerCanGetContent(
            string landing,
            bool authorized = true,
            int submissionid = 0)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var sut = landing.Equals("download-file-status") ?
                GetProvider(authorized, theFaker.Random.AlphaNumeric(10)).GetRequiredService<DataController>() :
                GetProvider(authorized).GetRequiredService<DataController>();
                var model = GetFormSubmissionModel(submissionid);
                var payload = GetPayload(submissionid);
                model.Payload = payload;
                if (submissionid == 103) model.FormName = string.Empty;
                if (submissionid == 200) model.Payload = theFaker.Random.Guid().ToString();
                IActionResult? result = landing switch
                {
                    "session-check" => sut.Check(model),
                    "submit" => await sut.Submit(model),
                    "fetch" => await sut.Fetch(model),
                    "filter-status" => sut.Filter(model),
                    "download-verify" => await sut.VerifyDownload(model),
                    "download-file-status" => sut.DownloadCompleted(),
                    "reset-cache" => await sut.ResetCache(model),
                    _ => null
                };
                Assert.NotNull(result);
            });
            Assert.Null(error);
        }

        private static FormSubmissionModel GetFormSubmissionModel(int modelId)
        {
            var category = 0;
            if (modelId >= 100 && modelId < 150) category = 100;
            if (modelId >= 150 && modelId < 200) category = 150;
            if (modelId >= 200 && modelId < 300) category = 200;
            if (modelId >= 300 && modelId < 400) category = 300;
            if (modelId >= 400 && modelId < 500) category = 400;
            if (modelId >= 500 && modelId < 600) category = 500;
            if (modelId >= 600 && modelId < 700) category = 600;
            var formName = category switch
            {
                0 => "check-session",
                100 => "form-login",
                150 => "form-register",
                200 => "mailbox",
                300 => "history-filter",
                400 => "check-download",
                600 => "form-cache-manager",
                _ => string.Empty
            };
            return new FormSubmissionModel { FormName = formName };
        }

        private static string GetPayload(int modelId)
        {
            if (modelId >= 700 || modelId < 0) return string.Empty;
            var payload = string.Empty;
            if (modelId == 0) payload = (new { id = 10, location = "home" }).ToJsonString();
            if (modelId == 1) payload = (new { id = 10, location = "my-account" }).ToJsonString();
            if (modelId == 2) payload = (new { id = 10, location = "invoice" }).ToJsonString();
            if (modelId == 3) payload = (new { id = 10, location = "correspondence" }).ToJsonString();
            if (modelId == 4 || modelId == 104) payload = (new { id = 10, location = "search" }).ToJsonString();
            if (modelId == 5) payload = (new { unmapped = 10, response = "correspondence" }).ToJsonString();
            var fkr = theFaker;
            var pword = string.Concat(fkr.Random.AlphaNumeric(10), fkr.Random.AlphaNumeric(10).ToUpper());
            if (modelId == 100) payload = new FormLoginModel { UserName = fkr.Person.Email, Password = pword }.ToJsonString();
            if (modelId == 101) payload = new FormLoginModel { UserName = fkr.Person.Email }.ToJsonString();
            if (modelId == 102) payload = new FormLoginModel { Password = pword }.ToJsonString();
            if (modelId == 103) payload = new FormLoginModel().ToJsonString();
            if (modelId == 150) payload = new FormRegistrationModel
            {
                Email = fkr.Person.Email,
                UserName = fkr.Person.FirstName,
                Password = pword,
                PasswordConfirmation = pword,
            }.ToJsonString();
            if (modelId == 151) payload = new FormRegistrationModel
            {
                UserName = fkr.Person.FirstName,
                Password = pword,
                PasswordConfirmation = pword,
            }.ToJsonString();
            if (modelId == 152) payload = new FormRegistrationModel
            {
                Email = fkr.Person.Email,
                Password = pword,
                PasswordConfirmation = pword,
            }.ToJsonString();
            if (modelId == 153) payload = new FormRegistrationModel
            {
                Email = fkr.Person.Email,
                UserName = fkr.Person.FirstName,
                PasswordConfirmation = pword,
            }.ToJsonString();
            if (modelId == 154) payload = new FormRegistrationModel
            {
                Email = fkr.Person.Email,
                UserName = fkr.Person.FirstName,
                Password = pword,
            }.ToJsonString();
            if (modelId == 155) payload = new FormRegistrationModel
            {
                Email = fkr.Person.Email,
                UserName = fkr.Person.FirstName,
                Password = pword,
                PasswordConfirmation = fkr.Random.AlphaNumeric(15),
            }.ToJsonString();
            if (modelId == 300) payload =
                    MockObjectProvider.GetSingle<FormStatusFilter>().ToJsonString();
            if (modelId == 400) payload =
                    (new { Id = theFaker.Random.Guid().ToString() }).ToJsonString();
            if (modelId == 600) payload =
                    new CacheUpdateRequest { Name = "correspondence" }.ToJsonString();
            if (modelId == 601) payload =
                    new CacheUpdateRequest { Name = "history" }.ToJsonString();
            if (modelId == 602) payload =
                    new CacheUpdateRequest { Name = "identity" }.ToJsonString();
            if (modelId == 603) payload =
                    new CacheUpdateRequest { Name = "not-mapped" }.ToJsonString();

            return payload;
        }

        private static readonly Faker theFaker = new();
    }
}