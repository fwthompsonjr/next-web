using next.core.entities;
using next.core.interfaces;

namespace next.core.implementations
{
    internal class HistoryReader : IHistoryReader
    {
        public async Task<string?> GetHistory(IPermissionApi? api, UserBo? user)
        {
            if (api == null || user == null || !user.IsAuthenicated) return null;
            var payload = GetPayload();
            var response = await api.Post("search-get-history", payload, user);
            if (response == null || response.StatusCode != 200) return null;
            return response.Message;
        }

        private static object? _payload;
        private static object GetPayload()
        {
            if (_payload != null) return _payload;
            _payload = new
            {
                id = Guid.NewGuid().ToString(),
                name = "legallead.permissions.api"
            };
            return _payload;
        }
    }
}
