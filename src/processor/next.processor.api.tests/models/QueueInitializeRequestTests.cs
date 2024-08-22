using Bogus;
using next.processor.api.extensions;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class QueueInitializeRequestTests
    {
        [Fact]
        public void QueueInitializeRequestCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new QueueInitializeRequest();
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueueInitializeRequestCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate(10);
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueueInitializeRequestFieldsCanGetAndSet()
        {
            var error = Record.Exception(() =>
            {
                var collection = faker.Generate(2);
                var a = collection[0];
                var b = collection[1];
                Assert.NotEqual(a.MachineName, b.MachineName);
                Assert.NotEqual(a.Message, b.Message);
                Assert.NotEqual(a.Items, b.Items);
                _ = a.IsValid();
                _ = a.Serialize();
            });
            Assert.Null(error);
        }


        [Fact]
        public void QueueInitializeRequestCanSerialize()
        {
            var error = Record.Exception(() =>
            {
                var collection = faker.Generate(20);
                collection.ForEach(c =>
                {
                    var items = c.Items;
                    items.ForEach(i => { if (items.IndexOf(i) % 2 == 1) { i.Id = string.Empty; } });
                    _ = c.Serialize();
                });
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(11)]
        [InlineData(12)]
        [InlineData(20)]
        [InlineData(21)]
        [InlineData(30)]
        [InlineData(40)]
        public void QueueInitializeRequestCanBeValidated(int failureId = 0)
        {
            int[] failures = [0, 1, 10, 11, 20, 21, 30];
            var error = Record.Exception(() =>
            {
                var fkr = new Faker();
                var selection = faker.Generate();
                selection.MachineName = failureId switch
                {
                    0 => string.Empty,
                    1 => fkr.Random.AlphaNumeric(300),
                    _ => selection.MachineName
                };
                selection.Message = failureId switch
                {
                    10 => string.Empty,
                    11 => fkr.Random.AlphaNumeric(300),
                    _ => selection.Message
                };
                selection.StatusId = failureId switch
                {
                    20 => null,
                    21 => fkr.Random.Int(5, 10),
                    _ => selection.StatusId
                };
                if (failureId == 30) selection.Items.Clear();
                var expected = !failures.Contains(failureId);
                var actual = selection.IsValid();
                Assert.Equal(expected, actual);
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueueInitializeRequestItem> faker1 =
            new Faker<QueueInitializeRequestItem>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString());

        private static readonly Faker<QueueInitializeRequest> faker =
            new Faker<QueueInitializeRequest>()
            .RuleFor(x => x.MachineName, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Message, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StatusId, y => y.Random.Int(-1, 2))
            .RuleFor(x => x.Items, y =>
            {
                var n = y.Random.Int(10, 20);
                return faker1.Generate(n);
            });
    }

}
