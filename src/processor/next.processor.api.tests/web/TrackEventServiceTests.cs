using Bogus;
using next.processor.api.models;

namespace next.processor.api.tests.web
{
    public class TrackEventServiceTests
    {
        private static readonly Faker<TrackEventModel> faker
            = new Faker<TrackEventModel>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(20))
            .RuleFor(x => x.Content, y => y.Lorem.Paragraphs(4))
            .RuleFor(x => x.ExpirationDate, y => y.Date.Recent(120));

        [Fact]
        public void ServiceContainsModels()
        {
            var error = Record.Exception(() =>
            {
                _ = TrackEventService.Models;
            });
            Assert.Null(error);
        }

        [Fact]
        public void ServiceCanAddOrUpdate()
        {
            lock (_locking)
            {
                try
                {
                    var error = Record.Exception(() =>
                    {
                        var models = faker.Generate(2);
                        var keyName = models[0].Name;
                        var expiration = TimeSpan.FromSeconds(60);
                        models.ForEach(m =>
                        {
                            var keyValue = m.Content;
                            TrackEventService.AddOrUpdate(keyName, keyValue, expiration);
                            var actual = TrackEventService.Get(keyName);
                            Assert.Equal(keyValue, actual);
                        });
                        TrackEventService.Expire();
                    });
                    Assert.Null(error);
                }
                finally
                {
                    TrackEventService.Clear();
                }
            }
        }

        [Fact]
        public void ServiceShouldNotFindMissingKey()
        {
            lock (_locking)
            {
                var error = Record.Exception(() =>
                {
                    var model = faker.Generate();
                    var keyName = model.Name;
                    var actual = TrackEventService.Get(keyName);
                    Assert.True(string.IsNullOrEmpty(actual));
                    TrackEventService.Expire();
                });
                Assert.Null(error);
            }
        }

        [Fact]
        public void ServiceCanExpire()
        {
            lock (_locking)
            {
                try
                {
                    var error = Record.Exception(() =>
                    {
                        var models = faker.Generate(10);
                        var expiration = TimeSpan.FromSeconds(-60);
                        models.ForEach(m =>
                        {
                            var keyName = m.Name;
                            var keyValue = m.Content;
                            TrackEventService.AddOrUpdate(keyName, keyValue, expiration);
                            var actual = TrackEventService.Get(keyName);
                            Assert.Equal(keyValue, actual);
                            Assert.True(TrackEventService.Exists(keyName));
                        });
                        TrackEventService.Expire();
                    });
                    Assert.Null(error);
                }
                finally
                {
                    TrackEventService.Clear();
                }
            }
        }

        private static readonly object _locking = new();
    }
}