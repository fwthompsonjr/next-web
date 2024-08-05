using next.web.core.extensions;
using legallead.desktop.interfaces;
using next.web.core.interfaces;
namespace next.web.core.services
{
    internal class ReMapStyles(IContentParser parser) : IReMapContent
    {
        protected string Index { get; } = DateTime.UtcNow.Ticks.ToString();
        protected IContentParser Parser { get; } = parser;

        public virtual string Map(string source)
        {
            const string find = "//link[@name]";
            var doc = source.ToHtml();
            var node = doc.DocumentNode;
            var links = node.SelectNodes(find).ToList();
            links.ForEach(link =>
            {
                var attr = link.Attributes.ToList().Find(a => a.Name == "href");
                if (attr != null && attr.Value.StartsWith('/'))
                {
                    var current = attr.Value.Split('?')[0];
                    var final = $"{current}?id={Index}";
                    attr.Value = final;
                }
            });
            return Parser.BeautfyHTML(node.OuterHtml);
        }
    }
}
