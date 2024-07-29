using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class CoreConfigurationModelTests
    {
        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                var sut = new CoreConfigurationModel();
                var config = sut.GetConfiguration;
                Assert.NotNull(config);
            });
            Assert.Null(error);
        }
    }
}
