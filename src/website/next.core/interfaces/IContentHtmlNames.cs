using next.core.entities;

namespace next.core.interfaces
{
    internal interface IContentHtmlNames
    {
        List<ContentHtml> ContentNames { get; }
        List<string> Names { get; }
        ISearchBuilder? SearchUi { get; set; }

        ContentHtml? GetContent(string name);

        Stream GetContentStream(string name);

        bool IsValid(string name);
    }
}