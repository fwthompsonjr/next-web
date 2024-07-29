using Bogus;
using next.web.Models;

namespace next.web.tests
{
    public class ErrorViewModelTests
    {
        private static readonly Faker<ErrorViewModel> faker =
            new Faker<ErrorViewModel>()
                .RuleFor(x => x.RequestId, y => y.Random.Guid().ToString());

        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                var obj = new ErrorViewModel();
                Assert.Null(obj.RequestId);
                Assert.False(obj.ShowRequestId);
            });
            Assert.Null(error);
        }
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var obj = faker.Generate();
                Assert.NotNull(obj.RequestId);
                Assert.True(obj.ShowRequestId);
            });
            Assert.Null(error);
        }
    }
}
