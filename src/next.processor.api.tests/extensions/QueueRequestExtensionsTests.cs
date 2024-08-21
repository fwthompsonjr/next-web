using Bogus;
using next.processor.api.extensions;
using next.processor.api.models;

namespace next.processor.api.tests.extensions
{
    public class QueueRequestExtensionsTests
    {
        [Fact]
        public void ServiceCanConvertObjectToString()
        {
            var obj = faker.Generate();
            var result = obj.ToJsonString();
            Assert.False(string.IsNullOrEmpty(result));
        }
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void ServiceCanDeserialize(int index)
        {
            var obj = faker.Generate(4);
            var result = obj.ToJsonString();
            if (string.IsNullOrEmpty(result)) return;
            if (index == 0) result = result.Replace('{', '>');
            var restored = result.ToInstance<List<QueueUpdateRequest>>();
            if (index == 0) Assert.Null(restored);
            else Assert.NotNull(restored);
        }


        private static readonly Faker<QueueUpdateRequest> faker =
            new Faker<QueueUpdateRequest>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.SearchId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Message, y => y.Random.AlphaNumeric(250))
            .RuleFor(x => x.StatusId, y => y.Random.Int(-1, 2));
    }
}
