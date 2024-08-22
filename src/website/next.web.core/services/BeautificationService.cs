using AngleSharp.Html;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using HtmlAgilityPack;
using next.web.core.interfaces;
using System.Text;

namespace next.web.core.services
{
    internal class BeautificationService : IBeautificationService
    {
        public string BeautfyHTML(string html)
        {
            string text = Parse(html);
            if (!text.Contains("<html>"))
            {
                return text;
            }

            return StandardizeDocument(text);
        }

        private sealed class IndentInnerText(HtmlNode node)
        {
            private readonly HtmlNode _node = node;

            public void Standardize()
            {
                int num = 20;
                string innerText = _node.InnerText;
                if (string.IsNullOrWhiteSpace(innerText.Trim()))
                {
                    return;
                }

                var stringBuilder = new StringBuilder(Environment.NewLine);
                using var stringReader = new StringReader(innerText);
                int num2 = num;
                for (string? text = stringReader.ReadLine(); text != null; text = stringReader.ReadLine())
                {
                    string text2 = text.Trim();
                    if (!string.IsNullOrEmpty(text2))
                    {
                        if (text2.Contains('{'))
                        {
                            num2 += 5;
                        }

                        string value = PadString(num2, text2);
                        stringBuilder.AppendLine(value);
                        if (text2.Contains('}'))
                        {
                            num2 -= 5;
                        }

                        if (num2 < num)
                        {
                            num2 = num;
                        }
                    }
                }

                stringBuilder.AppendLine();
                _node.InnerHtml = stringBuilder.ToString();
            }

            private static string PadString(int left, string source)
            {
                return new string(' ', left) + source + " ";
            }
        }

        private static string Parse(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return html;
            }

            IHtmlDocument htmlDocument = new HtmlParser().ParseDocument(html);
            using var stringWriter = new StringWriter();
            htmlDocument.ToHtml(stringWriter, new PrettyMarkupFormatter
            {
                Indentation = "\t",
                NewLine = "\n"
            });
            return stringWriter.ToString();
        }

        private static string StandardizeDocument(string html)
        {
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);
            var list = htmlDocument.DocumentNode.SelectNodes("//script")?.ToList();
            var list2 = htmlDocument.DocumentNode.SelectNodes("//style")?.ToList();
            list ??= [];

            if (list2 != null && list2.Count != 0)
            {
                list.AddRange(list2);
            }

            list.ForEach(delegate (HtmlNode s)
            {
                new IndentInnerText(s).Standardize();
            });
            return Parse(htmlDocument.DocumentNode.OuterHtml);
        }
    }
}
