using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class ErrorStatusMessageTests
    {
        private static readonly Faker<ErrorStatusMessage> faker =
            new Faker<ErrorStatusMessage>()
            .RuleFor(x => x.Id, y => y.Random.Int(100, 5000).ToString())
            .RuleFor(x => x.IsDefault, y => y.Random.Bool())
            .RuleFor(x => x.Code, y => y.Company.CompanyName())
            .RuleFor(x => x.Message, y =>
            {
                var list = new List<string>();
                int count = y.Random.Int(0, 3);
                for (var i = 0; i < count; i++)
                {
                    list.Add(y.Hacker.Phrase());
                }
                return list.ToArray();
            });

        [Fact]
        public void ErrorStatusMessageCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new ErrorStatusMessage();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ErrorStatusMessageCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Id = items[1].Id;
            Assert.Equal(items[1].Id, items[0].Id);
        }

        [Fact]
        public void ErrorStatusMessageCanUpdateIsDefault()
        {
            var items = faker.Generate(2);
            items[0].IsDefault = items[1].IsDefault;
            Assert.Equal(items[1].IsDefault, items[0].IsDefault);
        }

        [Fact]
        public void ErrorStatusMessageCanUpdateCode()
        {
            var items = faker.Generate(2);
            items[0].Code = items[1].Code;
            Assert.Equal(items[1].Code, items[0].Code);
        }

        [Fact]
        public void ErrorStatusMessageCanUpdateMessage()
        {
            var items = faker.Generate(2);
            items[0].Message = items[1].Message;
            Assert.Equal(items[1].Message, items[0].Message);
        }

        [Fact]
        public void ErrorStatusMessageCanGetDescription()
        {
            var items = faker.Generate(10);
            items.ForEach(i =>
            {
                var expected = i.Message.Length == 0;
                var text = i.Description;
                var actual = text.Length == 0;
                Assert.Equal(expected, actual);
            });
        }

        [Fact]
        public void ErrorStatusMessageCanGetMessages()
        {
            var items = faker.Generate(3);
            items.ForEach(i =>
            {
                var messages = ErrorStatusMessage.GetMessages();
                Assert.NotNull(messages);
                Assert.NotEmpty(messages);
            });
        }
    }
}