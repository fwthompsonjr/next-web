using next.core.entities;
using next.core.interfaces;

namespace next.web.core.interfaces
{
    public interface IAccountMapService
    {
        string GetHtml(string content, string viewName);
        string Headings(string content, string viewName);
        string Modals(string content);
        string Scripts(string content);
        string Shell(string content);
        Task<string> Transform(string html);
    }
}
