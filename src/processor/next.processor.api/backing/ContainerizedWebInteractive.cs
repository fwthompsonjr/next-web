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

        public WebFetchResult Temporary()
        {

            var fallbak = new WebFetchResult();
            if (interactive is not WebInteractive mysearch) return fallbak;
            XmlContentHolder results = new SettingsManager().GetOutput(mysearch);
            List<HLinkDataRow> cases = WebUtilities.GetCases(mysearch);
            return fallbak;
        }
    }
}
