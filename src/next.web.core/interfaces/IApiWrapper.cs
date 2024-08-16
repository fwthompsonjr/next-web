using Microsoft.AspNetCore.Http;
using next.web.core.models;

namespace next.web
{
    public interface IApiWrapper
    {
        Task<ApiAnswer> Get(string name);
        Task<ApiAnswer> Get(string name, ISession session);
        Task<ApiAnswer> Get(string name, Dictionary<string, string> parameters);
        Task<ApiAnswer> Get(string name, ISession session, Dictionary<string, string> parameters);
        Task<ApiAnswer> Post(string name, object payload, ISession session, string? userjs = null);
    }
}
