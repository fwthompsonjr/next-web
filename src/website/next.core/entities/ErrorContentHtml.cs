using HtmlAgilityPack;
using next.core.implementations;

namespace next.core.entities
{
    internal class ErrorContentHtml : ContentHtml
    {
        private static readonly object locker = new();
        public int StatusCode { get; set; }
        public bool IsDefault { get; set; }
        public ErrorStatusMessage StatusMessage { get; set; } = DefaultMessage;

        public static List<ErrorContentHtml> ErrorContentList()
        {
            if (_list != null) return _list;
            lock (locker)
            {
                _list = new List<ErrorContentHtml>();
                var messages = ErrorStatusMessage.GetMessages();
                messages.ForEach(m =>
                {
                    var item = new ErrorContentHtml
                    {
                        IsDefault = m.IsDefault.GetValueOrDefault(),
                        StatusCode = Convert.ToInt32(m.Id),
                        StatusMessage = m,
                        Index = (messages.IndexOf(m) * 10) + 5000,
                        Name = "Error",
                        Content = MapContent(m)
                    };
                    _list.Add(item);
                });
                return _list;
            }
        }

        private static string? _errorContent;
        private static ErrorStatusMessage? _dfStatusMessage;
        private static List<ErrorContentHtml>? _list;

        private static string ErrContent => _errorContent ??= GetErrorContent();
        private static ErrorStatusMessage DefaultMessage => _dfStatusMessage ??= GetDefaultMessage();

        private static ErrorStatusMessage GetDefaultMessage()
        {
            var messages = ErrorStatusMessage.GetMessages();
            return messages.Find(a => a.IsDefault.GetValueOrDefault()) ?? new();
        }

        private static string MapContent(ErrorStatusMessage m)
        {
            var source = ErrContent;
            var doc = new HtmlDocument();
            doc.LoadHtml(source);
            var find = new[] {
                "//*[@id=\"errorbox-error-code\"]",
                "//*[@id=\"errorbox-error-text\"]",
                "//*[@id=\"errorbox-error-text-description\"]" };
            var values = new[]
            {
                m.Id.ToString(),
                m.Code,
                m.Description
            };
            var elements = find.Select((s, index) =>
                new
                {
                    text = values[index],
                    element = doc.DocumentNode.SelectSingleNode(s)
                }).ToList();

            elements.ForEach(e =>
            {
                if (e.element != null) { e.element.InnerHtml = e.text; }
            });

            var html = doc.DocumentNode.OuterHtml;
            html = ContentHtmlNames.CommonReplacement(html);
            return html;
        }

        private static string GetErrorContent()
        {
            return Properties.Resources.error_html;
        }
    }
}