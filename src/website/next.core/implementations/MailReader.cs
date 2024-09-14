using next.core.entities;
using next.core.interfaces;

namespace next.core.implementations
{
    internal class MailReader : IMailReader
    {
        public string? GetBody(IPermissionApi? api, UserBo? user, string messageId)
        {
            if (api == null || user == null || !user.IsAuthenicated) return null;
            if (!Guid.TryParse(messageId, out var _)) return null;
            var payload = new
            {
                MessageId = messageId,
                RequestType = "body"
            };
            var response = api.Post("message-body", payload, user).GetAwaiter().GetResult();
            if (response == null || response.StatusCode != 200) return null;
            return response.Message;
        }

        public string? GetCount(IPermissionApi? api, UserBo? user)
        {
            if (api == null || user == null || !user.IsAuthenicated) return null;
            var payload = new
            {
                RequestType = "count"
            };
            var response = api.Post("message-count", payload, user).GetAwaiter().GetResult();
            if (response == null || response.StatusCode != 200) return null;
            return response.Message;
        }

        public string? GetMessages(IPermissionApi? api, UserBo? user)
        {
            if (api == null || user == null || !user.IsAuthenicated) return null;
            var payload = new
            {
                RequestType = "messages"
            };
            var response = api.Post("message-list", payload, user).GetAwaiter().GetResult();
            if (response == null || response.StatusCode != 200) return null;
            return response.Message;
        }


    }
}
