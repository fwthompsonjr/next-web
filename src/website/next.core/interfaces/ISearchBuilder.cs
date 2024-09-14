using next.core.entities;

namespace next.core.interfaces
{
    internal interface ISearchBuilder
    {
        StateSearchConfiguration[]? GetConfiguration();

        string GetHtml();
    }
}