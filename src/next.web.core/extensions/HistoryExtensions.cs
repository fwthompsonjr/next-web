using legallead.desktop.interfaces;
using Microsoft.AspNetCore.Http;

namespace next.web.core.extensions
{
    internal static class HistoryExtensions
    {
        public static async Task<string> GetHistory(
            this ISession session,
            IPermissionApi api,
            string source
        )
        {
            var document = source.ToHtml();
            if (document == null) return source;
            var list = await session.RetrieveMail(api);
            if (list == null) return source;
            return document.DocumentNode.OuterHtml;
        }
    }
}
/*

*/