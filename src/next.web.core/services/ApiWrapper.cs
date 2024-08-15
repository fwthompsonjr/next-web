using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.extensions;
using next.web.core.models;
using System.Net;

namespace next.web.Services
{
    public class ApiWrapper : IApiWrapper
    {
        private readonly IPermissionApi permissionApi;
        internal ApiWrapper(IPermissionApi api)
        {
            permissionApi = api;
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

        public async Task<ApiAnswer> Post(string name, object payload, ISession session)
        {
            var user = session.GetUser();
            if (user == null) return NotAuthorizedResponse();
            var response = await permissionApi.Post(name, payload, user);
            return MapFrom(response);
        }

        internal static ApiResponse MapTo(ApiAnswer response)
        {
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


    }
}
