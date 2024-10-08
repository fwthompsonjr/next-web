using next.web.core.extensions;
using next.web.core.interfaces;
using next.web.core.models;

namespace next.web.core.services
{
    public class CountyAuthorizationService : ICountyAuthorizationService
    {
        public List<AuthorizedCountyModel> Models => GetModels();

        private static List<AuthorizedCountyModel> GetModels()
        {
            var tmp = modeljs.ToInstance<List<AuthorizedCountyModel>>() ?? [];
            return tmp;
        }

        private static readonly string modeljs = Properties.Resources.county_authenication_list;
    }
}
