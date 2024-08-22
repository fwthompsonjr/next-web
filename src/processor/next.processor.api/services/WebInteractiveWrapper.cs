using legallead.records.search.Classes;
using legallead.records.search.Models;
using next.processor.api.interfaces;

namespace next.processor.api.services
{
    public class WebInteractiveWrapper : IWebInteractiveWrapper
    {
        public WebFetchResult? Fetch(WebInteractive web)
        {
            return web.Fetch();
        }
    }
}
