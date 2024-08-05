using HtmlAgilityPack;
using next.web.core.interfaces;

namespace next.web.core.services
{
    internal class DocumentViewSearch : IDocumentView
    {
        protected virtual string ViewName => "search";

        public virtual string SetChildMenu(string html)
        {
            const string selector = "//div[@name='sub-menu-search-{0}']";
            var doc = ContentSanitizerBase.GetDocument(html);
            if (doc == null) return html;
            var node = doc.DocumentNode;
            names.ForEach(name =>
            {
                var remove = name != ViewName;
                var find = string.Format(selector, name);
                var element = node.SelectSingleNode(find);
                ToggleClass(element, attr, "sub-menu-selected", remove);
            });
            return node.OuterHtml;
        }

        public virtual string SetMenu(string html)
        {
            const string selector = "//*[@id='my-search-parent-option']";
            var doc = ContentSanitizerBase.GetDocument(html);
            if (doc == null) return html;
            var node = doc.DocumentNode;
            var element = node.SelectSingleNode(selector);
            ToggleClass(element, attr, "menu-selected", false);
            return node.OuterHtml;
        }

        public virtual string SetTab(string html)
        {
            const string actv = "active";
            string target = ViewName;
            const string selector = "//a[@name='subcontent-{0}']";
            const string dvselector = "//div[@name='subcontent-{0}']";
            var doc = ContentSanitizerBase.GetDocument(html);
            if (doc == null) return html;
            var node = doc.DocumentNode;
            names.ForEach(name =>
            {
                var tabselector = string.Format(selector, name);
                var divselector = string.Format(dvselector, name);
                var tab = node.SelectSingleNode(tabselector);
                var dv = node.SelectSingleNode(divselector);
                var remove = name != target;
                ToggleClass(tab, attr, actv, remove);
                ToggleClass(dv, attr, actv, remove);
            });
            return node.OuterHtml;
        }
        private const string attr = "class";
        private static readonly List<string> names = [.. "search,active,purchases".Split(',')];
        protected static void ToggleClass(HtmlNode? element, string attribute, string find, bool remove)
        {
            if (element == null) return;
            var attr = element.Attributes.FirstOrDefault(a => a.Name == attribute);
            if (attr == null) return;
            var cls = attr.Value.Split(' ').ToList();
            if (!remove && !cls.Contains(find)) cls.Add(find);
            if (remove && cls.Contains(find)) cls.Remove(find);
            attr.Value = string.Join(" ", cls);
        }
    }
}