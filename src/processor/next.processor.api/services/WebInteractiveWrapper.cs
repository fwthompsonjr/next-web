using legallead.records.search.Classes;
using legallead.records.search.Models;
using next.processor.api.backing;
using next.processor.api.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace next.processor.api.services
{
    [ExcludeFromCodeCoverage(Justification = "Wrapper class that prevents live internet requests in unit testing")]
    public class WebInteractiveWrapper : IWebInteractiveWrapper
    {
        public WebFetchResult? Fetch(WebInteractive web)
        {
            var containerized = new ContainerizedWebInteractive(web);
            return containerized.Fetch();
        }


    }
}
