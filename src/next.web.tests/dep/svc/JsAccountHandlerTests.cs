using next.web.core.models;
using next.web.core.services;

namespace next.web.tests.dep.svc
{
    public class JsAccountHandlerTests
    {
        [Theory]
        [InlineData(200)]
        public void HandlerCanBeCreated(int code)
        {
            var error = Record.Exception(() =>
            {
                var api = new MockAccountApi(code);
                _ = new JsAccountHandler(api);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(200, "profile_edit_contact_name")]
        [InlineData(200, "profile_edit_contact_address")]
        [InlineData(200, "profile_edit_contact_phone")]
        [InlineData(200, "frm_profile_email")]
        [InlineData(200, "permissions_change_password")]
        [InlineData(200, "permissions_set_discount")]
        [InlineData(200, "permissions_set_permission")]
        public void HandlerCanSetPayload(int code, string formName)
        {
            var error = Record.Exception(() =>
            {
                var api = new MockAccountApi(code);
                var json = GetPayload(formName);
                _ = new JsAccountHandler(api);
                Assert.False(string.IsNullOrEmpty(json));
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(200, "profile_edit_contact_name")]
        [InlineData(200, "profile_edit_contact_address")]
        [InlineData(200, "profile_edit_contact_phone")]
        [InlineData(200, "frm_profile_email")]
        [InlineData(200, "permissions_change_password")]
        [InlineData(200, "permissions_set_discount")]
        [InlineData(200, "permissions_set_permission")]
        public async Task HandlerCanSetPost(int code, string formName)
        {
            var session = MockUserSession.GetInstance();
            var error = await Record.ExceptionAsync(async () =>
            {
                var api = new MockAccountApi(code);
                var json = GetPayload(formName);
                var request = new FormSubmissionModel
                {
                    FormName = formName,
                    Payload = json
                };
                var svc = new JsAccountHandler(api);
                _ = await svc.Submit(request, session.MqSession.Object);
            });
            Assert.Null(error);
        }
        private static string GetPayload(string name)
        {
            return MockAccountApi.GetPayload(name);
        }
    }
}
