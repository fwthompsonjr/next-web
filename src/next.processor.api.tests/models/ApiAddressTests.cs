using Bogus;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class ApiAddressTests
    {
        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new ApiAddress();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(error);
        }

        [Fact]
        public void ModelHasExpectedFields()
        {
            var error = Record.Exception(() =>
            {
                var test = faker.Generate(2);
                var a = test[0];
                var b = test[1];
                Assert.NotEqual(a.Name, b.Name);
                Assert.NotEqual(a.Address, b.Address);
            });
            Assert.Null(error);
        }

        private static readonly Faker<ApiAddress> faker =
            new Faker<ApiAddress>()
            .RuleFor(x => x.Name, y => y.Random.Int(1, 500000).ToString())
            .RuleFor(x => x.Address, y => y.Hacker.Phrase());
    }
}