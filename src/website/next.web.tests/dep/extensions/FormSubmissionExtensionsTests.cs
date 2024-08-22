using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Moq;
using next.web.core.extensions;
using next.web.core.models;

namespace next.web.tests.dep.extensions
{
    public class FormSubmissionExtensionsTests
    {

        [Theory]
        [InlineData("", 0)]
        [InlineData("", 1)]
        [InlineData("", 2)]
        [InlineData("profile_edit_contact_name", 0)]
        [InlineData("profile_edit_contact_name", 1)]
        [InlineData("profile_edit_contact_name", 2)]
        [InlineData("profile_edit_contact_address", 0)]
        [InlineData("profile_edit_contact_address", 1)]
        [InlineData("profile_edit_contact_address", 2)]
        public void FormCanBeValidated(string formName, int testId)
        {
            var error = Record.Exception(() =>
            {
                var model = GetModel(formName);
                var request = GetRequest();
                var keys = GetKeys(formName);
                request.SetupGet(x => x.Form).Returns(keys);
                if (testId == 1)
                {
                    model.Payload = string.Empty;
                }
                if (testId == 2)
                {
                    model.FormName = string.Empty;
                }
                model.Validate(request.Object);
            });
            Assert.Null(error);
        }

        private static FormSubmissionModel GetModel(string formName)
        {
            var model = new FormSubmissionModel
            {
                FormName = formName,
                Payload = MockAccountApi.GetPayload(formName)
            };
            return model;
        }

        private static FormCollection GetKeys(string formName)
        {
            var obj = GetModel(formName).ToJsonString();
            var model = new Dictionary<string, StringValues>
            {
                { "formName", new StringValues(formName) },
                { "payload", new StringValues(MockAccountApi.GetPayload(formName)) },
                { "form", new StringValues(obj) }
            };
            return new FormCollection(model);
        }

        private static Mock<HttpRequest> GetRequest()
        {
            var request = new Mock<HttpRequest>();
            request.Setup(x => x.Scheme).Returns("http");
            request.Setup(x => x.Host).Returns(HostString.FromUriComponent("http://localhost:8080"));
            request.Setup(x => x.PathBase).Returns(PathString.FromUriComponent("/api"));
            return request;
        }
    }
}
