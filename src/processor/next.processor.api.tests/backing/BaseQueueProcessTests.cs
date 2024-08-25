using Bogus;
using next.processor.api.extensions;
using next.processor.api.models;

namespace next.processor.api.tests.backing
{
    public class BaseQueueProcessTests
    {
        [Fact]
        public void ServiceCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new MockQueueProcess();
            });
            Assert.Null(error);
        }
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServiceCanToggleSuccessFlag(bool expected)
        {
            var error = Record.Exception(() =>
            {
                using var sut = new MockQueueProcess();
                sut.WriteSuccess(expected);
                Assert.Equal(expected, sut.IsSuccess);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ServiceCanToggleIterateNextFlag(bool expected)
        {
            var error = Record.Exception(() =>
            {
                using var sut = new MockQueueProcess();
                sut.WriteIterateNext(expected);
                Assert.Equal(expected, sut.AllowIterateNext);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ServiceCanGetUserSearch(int payloadType)
        {
            var data = payloadType != 3 ? recordfaker.Generate() : null;
            if (data != null)
            {
                data.Payload = payloadType switch
                {
                    0 => null,
                    1 => string.Empty,
                    _ => data.Payload
                };
            }
            var error = Record.Exception(() =>
            {
                using var sut = new MockQueueProcess();
                _ = sut.TranslateToUserSearchRequest(data);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void ServiceCanGetSearch(int payloadType)
        {
            var data = payloadType != 3 ? recordfaker.Generate() : null;
            if (data != null)
            {
                data.Payload = payloadType switch
                {
                    0 => null,
                    1 => string.Empty,
                    _ => data.Payload
                };
            }
            var error = Record.Exception(() =>
            {
                using var sut = new MockQueueProcess();
                _ = sut.TranslateToSearchRequest(data);
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueueSearchItem> itemfaker =
            new Faker<QueueSearchItem>()
            .RuleFor(x => x.WebId, y => y.Random.Int(1, 500000))
            .RuleFor(x => x.State, y => y.Random.AlphaNumeric(35))
            .RuleFor(x => x.County, y => y.Random.AlphaNumeric(50))
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30).ToString())
            .RuleFor(x => x.EndDate, y => y.Date.Recent(90).ToString());

        private static readonly Faker<QueuedRecord> recordfaker =
            new Faker<QueuedRecord>()
            .RuleFor(x => x.Id, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.UserId, y => y.Random.Guid().ToString())
            .RuleFor(x => x.StartDate, y => y.Date.Recent(30))
            .RuleFor(x => x.EndDate, y => y.Date.Recent(60))
            .RuleFor(x => x.ExpectedRows, y => y.Random.Int(0, 50000))
            .RuleFor(x => x.CreateDate, y => y.Date.Recent(60))
            .FinishWith((a, b) => b.Payload = itemfaker.Generate().ToJsonString());
    }
}
