using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ApiResponseTests
    {
        private readonly Faker<ApiResponse> faker =
            new Faker<ApiResponse>()
            .RuleFor(x => x.StatusCode, y => y.Random.Int(1, 500))
            .RuleFor(x => x.Message, y => y.Company.CompanyName());

        [Fact]
        public void ApiResponseCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ApiResponse();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ApiResponseCanUpdateStatusCode()
        {
            var items = faker.Generate(2);
            items[0].StatusCode = items[1].StatusCode;
            Assert.Equal(items[1].StatusCode, items[0].StatusCode);
        }

        [Fact]
        public void ApiResponseCanUpdateMessage()
        {
            var items = faker.Generate(2);
            items[0].Message = items[1].Message;
            Assert.Equal(items[1].Message, items[0].Message);
        }
    }
}