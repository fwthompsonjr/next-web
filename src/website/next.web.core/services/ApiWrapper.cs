using next.core.entities;
using next.core.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;
using System.Net;

namespace next.web.Services
{
    public class ApiWrapper : IApiWrapper
    {
        private readonly IPermissionApi permissionApi;
        private readonly IBeautificationService parser;
        internal ApiWrapper(IPermissionApi api, IBeautificationService content)
        {
            permissionApi = api;
            parser = content;
        }

        public async Task<string> InjectHttpsRedirect(string content, ISession session)
        {
            const string landing = "setting-application-key";
            const string script = "<script name='app' src='/js/app.js'></script>";
            var payload = new { keyName = "HTTP_REDIRECT_ENABLED" };
            var response = await Post(landing, payload, session, GetUserJson());
            if (response == null || response.StatusCode != 200) return content;
            var model = response.Message.ToInstance<AppSettingModel>();
            if (model == null || !model.KeyValue.Equals("true", StringComparison.OrdinalIgnoreCase)) return content;
            var doc = content.ToHtml();
            var node = doc.DocumentNode;
            var body = node.SelectSingleNode("//body");
            var html = body.InnerHtml;
            html = string.Concat(html, Environment.NewLine, script);
            body.InnerHtml = html;
            var revised = node.OuterHtml;
            revised = parser.BeautfyHTML(revised);
            return revised;
        }
        public async Task<ApiAnswer> Get(string name)
        {
            var response = await permissionApi.Get(name);
            return MapFrom(response);
        }

        public async Task<ApiAnswer> Get(string name, ISession session)
        {
            var user = session.GetUser();
            if (user == null) return NotAuthorizedResponse();
            var response = await permissionApi.Get(name, user);
            return MapFrom(response);
        }

        public async Task<ApiAnswer> Get(string name, Dictionary<string, string> parameters)
        {
            var response = await permissionApi.Get(name, parameters);
            return MapFrom(response);
        }

        public async Task<ApiAnswer> Get(string name, ISession session, Dictionary<string, string> parameters)
        {
            var user = session.GetUser();
            if (user == null) return NotAuthorizedResponse();
            var response = await permissionApi.Get(name, user, parameters);
            return MapFrom(response);
        }

        public async Task<ApiAnswer> Post(string name, object payload, ISession session, string? userjs = null)
        {
            var jsuser =
                string.IsNullOrEmpty(userjs) ? null :
                userjs.ToInstance<UserBo>();
            var user = session.GetUser();
            user ??= jsuser;
            if (user == null) return NotAuthorizedResponse();
            var response = await permissionApi.Post(name, payload, user);
            return MapFrom(response);
        }

        internal static ApiResponse MapTo(ApiAnswer response)
        {
            response ??= NotAuthorizedResponse();
            return new()
            {
                Message = response.Message,
                StatusCode = response.StatusCode,
            };
        }

        private static ApiAnswer MapFrom(ApiResponse response)
        {
            return new()
            {
                Message = response.Message,
                StatusCode = response.StatusCode,
            };
        }

        private static ApiAnswer NotAuthorizedResponse()
        {
            return new()
            {
                Message = "Account is not authorized",
                StatusCode = (int)HttpStatusCode.Unauthorized,
            };
        }

        private static string GetUserJson()
        {
            var apps = new List<ApiContext>
            {
                new(){ Id = "d6450506-3479-4c02-92c7-de59f6e7091e", Name = "legallead.permissions.api" }
            }.ToArray();
            return new UserBo()
            {
                Applications = apps
            }.ToJsonString();
        }

    }
}
