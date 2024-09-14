using next.core.entities;

namespace next.core.interfaces
{
    internal interface IErrorContentProvider
    {
        List<ContentHtml> ContentNames { get; }
        List<int> Names { get; }

        ContentHtml? GetContent(int name);

        bool IsValid(int name);
    }
}