using HtmlAgilityPack;

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
        private static HtmlDocument ToDocument(this string content)
        {
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;
        }
    }
}
