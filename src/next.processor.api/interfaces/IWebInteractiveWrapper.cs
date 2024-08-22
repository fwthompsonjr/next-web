using legallead.records.search.Classes;
using legallead.records.search.Models;

namespace next.processor.api.interfaces
{
    public interface IWebInteractiveWrapper
    {
        WebFetchResult? Fetch(WebInteractive web);
    }
}
