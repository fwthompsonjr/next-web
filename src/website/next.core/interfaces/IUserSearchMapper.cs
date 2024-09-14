using HtmlAgilityPack;

namespace next.core.interfaces
{
    internal interface IUserSearchMapper
    {
        string Map(string? history, out int rows);
        string Map(IHistoryPersistence? persistence, string? history, out int rows);
        void SetCounty(IHistoryPersistence? persistence, HtmlNode? combo);
        void SetFilter(IHistoryPersistence? persistence, HtmlNode? combo);
    }
}
