using next.web.core.models;

namespace next.web.core.interfaces
{
    public interface ICountyAuthorizationService
    {
        public List<AuthorizedCountyModel> Models { get; }
    }
}
