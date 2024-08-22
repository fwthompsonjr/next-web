using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;
using next.web.core.models;
using next.web.Services;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.extensions
{
    internal static class GetOperations
    {

        public static async Task<string> GetUserId(this UserBo user, IPermissionApi api)
        {
            const string landing = "get-contact-index";
            var payload = new { RequestType = "UserId" };
            var response = await api.Post(landing, payload, user);
            if (response.StatusCode != 200) return string.Empty;
            var data = response.Message.ToInstance<ContactProfileResponse>();
            if (data == null ||
                string.IsNullOrEmpty(data.Message) ||
                !Guid.TryParse(data.Message, out var _)) return string.Empty;
            return data.Message;
        }

        public static async Task<string> GetUserId(this ISession session, IApiWrapper api)
        {
            const string landing = "get-contact-index";
            var payload = new { RequestType = "UserId" };
            var response = await api.Post(landing, payload, session);
            if (response.StatusCode != 200) return string.Empty;
            var data = response.Message.ToInstance<ContactProfileResponse>();
            if (data == null ||
                string.IsNullOrEmpty(data.Message) ||
                !Guid.TryParse(data.Message, out var _)) return string.Empty;
            return data.Message;
        }

        public static async Task<MailItemBody?> GetMailBody(this UserBo user, IPermissionApi api, string messageId)
        {
            if (!Guid.TryParse(messageId, out var _)) return null;
            var payload = new
            {
                MessageId = messageId,
                RequestType = "body"
            };
            var response = await api.Post("message-body", payload, user);
            if (response == null || response.StatusCode != 200) return null;
            return response.Message.ToInstance<MailItemBody>();
        }

        public static async Task<MailItemBody?> GetMailBody(this ISession session, IApiWrapper api, string messageId)
        {
            if (!Guid.TryParse(messageId, out var _)) return null;
            var payload = new
            {
                MessageId = messageId,
                RequestType = "body"
            };
            var response = await api.Post("message-body", payload, session);
            if (response == null || response.StatusCode != 200) return null;
            return response.Message.ToInstance<MailItemBody>();
        }


        public static async Task<UserIdentityBo> GetUserIdentity(this UserBo user, IPermissionApi api)
        {
            const string landing = "get-contact-identity";
            const string getname = "get-contact-detail";
            var payload = user.Applications?.FirstOrDefault() ?? new();
            var response = await api.Post(landing, payload, user);
            if (response.StatusCode != 200) return new();
            var data = response.Message.ToInstance<UserIdentityBo>() ?? new();
            if (!string.IsNullOrEmpty(data.RoleDescription)) data.RoleDescription = string.Empty;
            var request = new { RequestType = "Name" };
            response = await api.Post(getname, request, user);
            return GetIdentityResponse(response, data);
        }
        public static async Task<UserIdentityBo> GetUserIdentity(this ISession session, IApiWrapper api)
        {
            const string landing = "get-contact-identity";
            const string getname = "get-contact-detail";
            var user = session.GetUser() ?? new();
            var payload = user.Applications?.FirstOrDefault() ?? new();
            var response = ApiWrapper.MapTo(await api.Post(landing, payload, session));
            if (response.StatusCode != 200) return new();
            var data = response.Message.ToInstance<UserIdentityBo>() ?? new();
            if (!string.IsNullOrEmpty(data.RoleDescription)) data.RoleDescription = string.Empty;
            var request = new { RequestType = "Name" };
            response = ApiWrapper.MapTo(await api.Post(getname, request, session));
            return GetIdentityResponse(response, data);
        }


        [ExcludeFromCodeCoverage]
        private static UserIdentityBo GetIdentityResponse(ApiResponse response, UserIdentityBo data)
        {
            if (response.StatusCode != 200) return data;
            var profile = response.Message.ToInstance<List<ContactProfileResponse>>();
            if (profile == null) return data;
            var item = profile.Find(a => a.ResponseType.Equals("Name"))?.Data;
            if (string.IsNullOrEmpty(item)) return data;
            var detail = item.ToInstance<List<ContactName>>();
            if (detail == null) return data;
            var fname = detail.Find(x => x.NameType == "First")?.Name ?? string.Empty;
            var lname = detail.Find(x => x.NameType == "Last")?.Name ?? string.Empty;
            data.FullName = $"{fname} {lname}".Trim();
            return data;
        }
    }
}
