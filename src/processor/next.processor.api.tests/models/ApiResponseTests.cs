using Bogus;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class ApiResponseTests
    {
        [Fact]
        public void ModelCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new ApiResponse();
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
                Assert.NotEqual(a.StatusCode, b.StatusCode);
                Assert.NotEqual(a.Message, b.Message);
            });
            Assert.Null(error);
        }

        private static Faker<ApiResponse> faker =
            new Faker<ApiResponse>()
            .RuleFor(x => x.StatusCode, y => y.Random.Int(1, 500000))
            .RuleFor(x => x.Message, y => y.Hacker.Phrase());
    }
}
