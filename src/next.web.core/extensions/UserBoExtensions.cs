using legallead.desktop.entities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using next.web.core.models;
using next.web.core.util;
using System.Text;

namespace next.web.core.extensions
{
    internal static class UserBoExtensions
    {
        public static void Save(this UserContextBo user, ISession session)
        {
            var json = JsonConvert.SerializeObject(user);
            var exists = session.Keys.ToList().Exists(x => x == SessionKeyNames.UserBo);
            if (exists) { session.Remove(SessionKeyNames.UserBo); }
            session.Set(SessionKeyNames.UserBo, Encoding.UTF8.GetBytes(json));
        }
        public static UserBo ToUserBo(this UserContextBo user)
        {
            var app = new List<ApiContext> { new() { Id = user.AppId, Name = user.AppName } }.ToArray();
            var token = new AccessTokenBo
            {
                AccessToken = user.Token ?? string.Empty,
                Expires = user.Expires ?? DateTime.MinValue,
                RefreshToken = user.RefreshToken ?? string.Empty
            };
            var response = new UserBo
            {
                Applications = app,
                Token = token,
                UserName = user.UserName,
            };
            return response;
        }
    }
}
