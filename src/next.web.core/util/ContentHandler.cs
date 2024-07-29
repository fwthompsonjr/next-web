using legallead.desktop.entities;
using legallead.desktop.interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace next.web.core.util
{
    internal static class ContentHandler
    {
        internal static ContentHtml? GetLocalContent(string name)
        {
            var provider = AppContainer.ServiceProvider;
            var contentProvider = ContentProvider.LocalContentProvider;
            if (contentProvider.SearchUi == null)
            {
                var searchUI = provider?.GetService<ISearchBuilder>();
                if (searchUI != null)
                {
                    contentProvider.SearchUi = searchUI;
                }
            }
            var raw = contentProvider.GetContent(name);
            if (raw == null) return null;
            var beutifier = provider?.GetRequiredService<IContentParser>();
            if (beutifier == null) return raw;
            var text = new StringBuilder(raw.Content);
            common.Keys.ToList().ForEach(key =>
            {
                var replacement = common[key];
                text.Replace(key, replacement);
            });
            raw.Content = beutifier.BeautfyHTML(text.ToString());
            return raw;
        }
        private static readonly string appName = CoreMetaData.GetKeyOrDefault("app.meta:name", "Oxford Legal Lead");
        private static readonly string appCode = CoreMetaData.GetKeyOrDefault("app.meta:prefix", "oxford.leads.web");
        private static readonly Dictionary<string, string> common = new() {
            { "Legal Lead", appName },
            { $"{appName}s UI", appName },
            { "legallead.ui", appCode },
            { "(c)", "&copy;" }
        };
    }
}
