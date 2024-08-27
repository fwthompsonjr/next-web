using legallead.records.search.Classes;
using legallead.records.search.Models;

namespace next.processor.api.backing
{
    public class ContainerizedWebInteractive(IWebInteractive web)
    {
        private readonly IWebInteractive interactive = web;

        public virtual WebFetchResult Fetch()
        {
            return interactive.Fetch();
        }
    }
}
