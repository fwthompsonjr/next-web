using Microsoft.AspNetCore.Http;
using next.web.core.models;
using System.Net;

namespace next.web.Services
{
    public class UnavailableApiWrapper : IApiWrapper
    {
        public Task<ApiAnswer> Get(string name)
        {
            var response = NotAvailableResponse();
            return Task.FromResult(response);
        }

        public Task<ApiAnswer> Get(string name, ISession session)
        {
            var response = NotAvailableResponse();
            return Task.FromResult(response);
        }

        public Task<ApiAnswer> Get(string name, Dictionary<string, string> parameters)
        {
            var response = NotAvailableResponse();
            return Task.FromResult(response);
        }

        public Task<ApiAnswer> Get(string name, ISession session, Dictionary<string, string> parameters)
        {
            var response = NotAvailableResponse();
            return Task.FromResult(response);
        }

        public Task<ApiAnswer> Post(string name, object payload, ISession session, string? userjs = null)
        {
            var response = NotAvailableResponse();
            return Task.FromResult(response);
        }
        private static ApiAnswer NotAvailableResponse()
        {
            return new()
            {
                Message = "Service is not available",
                StatusCode = (int)HttpStatusCode.ServiceUnavailable,
            };
        }
    }
}
