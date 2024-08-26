using legallead.records.search.Classes;
using legallead.records.search.Models;
using Moq;
using next.processor.api.backing;

namespace next.processor.api.tests.backing
{
    public class ContainerizedWebTests
    {
        [Fact]
        public void ServiceCanFetch()
        {
            var interactive = new Mock<WebInteractive>();
            var result = new WebFetchResult();
            interactive.Setup(x => x.Fetch()).Returns(result);
            var service = new ContainerizedWebInteractive(interactive.Object);
            _ = service.Fetch();
            interactive.Verify(x => x.Fetch());
        }
    }
}
