using HtmlAgilityPack;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace next.processor.api.services
{
    public static class HtmlMapper
    {
        public static string Home(string content, string health = "Healthy")
        {
            var document = content.ToDocument();
            var node = document.DocumentNode;
            if (string.IsNullOrWhiteSpace(health)) health = "Healthy";
            var substitutions = new Dictionary<string, string>
            {
                { "//span[@name='detail-01-caption']", Environment.MachineName.ToUpper() },
                { "//span[@name='detail-02-caption']", DateTime.UtcNow.ToString("s") },
                { "//span[@name='detail-03-caption']", health }
            };
            var keys = substitutions.Keys.ToList();
            keys.ForEach(key =>
            {
                var indx = keys.IndexOf(key);
                var find = node.SelectSingleNode(key);
                if (find != null)
                {
                    find.InnerHtml = substitutions[key];
                    if (indx == 2) AlterNodeClass(find, health);
                }
            });
            return node.OuterHtml;
        }

        public static string Home(string content, Dictionary<string, object> substitutions)
        {
            const string find = "//table[@name='tb-detail']/tbody"; 
            var document = content.ToDocument();
            var node = document.DocumentNode;
            var tbody = node.SelectSingleNode(find);
            if (tbody == null) return node.OuterHtml;
            var tr = tbody.SelectSingleNode("tr");
            if (tr == null) return node.OuterHtml;
            var template = tr.OuterHtml.Replace("template-row", "detail-row");
            var builder = new StringBuilder();
            substitutions.Keys.ToList().ForEach(k =>
            {
                var data = Convert.ToString(substitutions[k]);
                var content = template.Replace("~0", k).Replace("~1", data);
                builder.AppendLine(content);
            });
            tbody.InnerHtml = builder.ToString();
            return node.OuterHtml;
        }

        private static void AlterNodeClass(HtmlNode node, string health)
        {
            const string cls = "class";
            const string secondary = "text-secondary";
            var clsname = health.ToLower() switch
            {
                "healthy" => "text-success",
                "degraded" => "text-warning",
                "unhealthy" => "text-danger",
                _ => secondary
            };
            AddOrUpdateAttribute(node, cls, secondary, clsname);
        }


        private static HtmlDocument ToDocument(this string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }

        [ExcludeFromCodeCoverage]
        private static void AddOrUpdateAttribute(HtmlNode node, string cls, string secondary, string clsname)
        {
            var clss = node.Attributes.FirstOrDefault(x => x.Name == cls);
            if (clss == null)
            {
                node.Attributes.Add(cls, clsname);
                return;
            }
            var items = clss.Value.Split(' ').ToList();
            items.Remove(secondary);
            items.Add(clsname);
            clss.Value = string.Join(" ", items);
        }
    }
}
