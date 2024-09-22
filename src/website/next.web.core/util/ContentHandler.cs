using Microsoft.Extensions.DependencyInjection;
using next.core.entities;
using next.core.interfaces;
using next.web.core.interfaces;

namespace next.web.core.util
{
    internal static class ContentHandler
    {
        internal static ContentHtml? GetLocalContent(string name)
        {
            if (AppContainer.ServiceProvider == null) AppContainer.Build();
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
            var cleaner = AppContainer.GetSanitizer(name);
            var html = cleaner?.Sanitize(raw.Content) ?? raw.Content;
            raw.Content = html;
            var beutifier = provider?.GetRequiredService<IBeautificationService>();
            if (beutifier == null) return raw;
            raw.Content = beutifier.BeautfyHTML(html);
            return raw;
        }
    }
}
