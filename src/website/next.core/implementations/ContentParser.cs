using AngleSharp.Html;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using next.core.interfaces;
using System.Text;

namespace next.core.implementations
{
    internal class ContentParser : IContentParser
    {
        public string BeautfyHTML(string html)
        {
            const string htmlTag = "<html>";
            var formatted = Parse(html);
            if (!formatted.Contains(htmlTag)) return formatted;
            return StandardizeDocument(formatted);
        }

        private static string Parse(string html)
        {
            if (string.IsNullOrWhiteSpace(html)) return html;
            var parser = new HtmlParser();
            var document = parser.ParseDocument(html);
            using var writer = new StringWriter();
            document.ToHtml(writer, new PrettyMarkupFormatter
            {
                Indentation = "\t",
                NewLine = "\n"
            });
            return writer.ToString();
        }

        private static string StandardizeDocument(string html)
        {
            const string scriptpath = "//script";
            const string stylepath = "//style";
            var document = new HtmlDocument();
            document.LoadHtml(html);
            var scripts = document.DocumentNode.SelectNodes(scriptpath)?.ToList();
            var styles = document.DocumentNode.SelectNodes(stylepath)?.ToList();
            scripts ??= new();
            if (styles != null && styles.Any()) { scripts.AddRange(styles); }
            scripts.ForEach(s =>
            {
                var indenter = new IndentInnerText(s);
                indenter.Standardize();
            });
            return Parse(document.DocumentNode.OuterHtml);
        }

        private sealed class IndentInnerText
        {
            private readonly HtmlNode _node;

            public IndentInnerText(HtmlNode node)
            {
                _node = node;
            }

            public void Standardize()
            {
                int basicIndent = 20;
                var text = _node.InnerText;
                var trimmed = text.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) return;
                var sb = new StringBuilder(Environment.NewLine);
                using var reader = new StringReader(text);
                var currentLevel = basicIndent;
                var line = reader.ReadLine();
                while (line != null)
                {
                    var current = line.Trim();
                    if (!string.IsNullOrEmpty(current))
                    {
                        if (current.Contains('{')) { currentLevel += 5; }
                        var data = PadString(currentLevel, current);
                        sb.AppendLine(data);
                        if (current.Contains('}')) { currentLevel -= 5; }
                        if (currentLevel < basicIndent) currentLevel = basicIndent;
                    }
                    line = reader.ReadLine();
                }
                sb.AppendLine();
                _node.InnerHtml = sb.ToString();
            }

            private static string PadString(int left, string source)
            {
                var leftPadding = new string(' ', left);
                return $"{leftPadding}{source} ";
            }
        }
    }
}