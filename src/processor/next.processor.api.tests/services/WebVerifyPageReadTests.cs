using Bogus;
using legallead.records.search.Classes;
using legallead.records.search.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using next.processor.api.services;
using next.processor.api.utility;

namespace next.processor.api.tests.services
{
    public class WebVerifyPageReadTests
    {
        [Theory]
        [InlineData(-1, false)]
        [InlineData(0, true)]
        [InlineData(1, true)]
        [InlineData(2, true)]
        [InlineData(3, true)]
        [InlineData(4, true)]
        [InlineData(5, false)]
        public void ServiceCanGetInteractive(int index, bool expected)
        {
            var service = new MockPageReader();
            var actual = service.GetWebInteraction(index) != null;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void ServiceCanSetWebIndex(int index)
        {
            var service = new MockPageReader();
            service.SetIndex(index);
            Assert.Equal(index, service.ExternalId);
        }
        [Theory]
        [InlineData(-1)]
        [InlineData(10)]
        public async Task ServiceShouldReturnFalseForUnmappedIndexAsync(int index)
        {
            var service = new MockPageReader();
            service.SetIndex(index);
            var actual = await service.InstallAsync();
            Assert.False(actual);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(0, 0)]
        [InlineData(0, 10)]
        public async Task ServiceShouldFetchAsync(int index, int responseId = 1)
        {
            var service = new MockPageReader(true);
            var mock = service.Mock;
            WebFetchResult data = responseId switch
            {
                1 => new WebFetchResult
                {
                    PeopleList = [new(), new()]
                },
                _ => new WebFetchResult()
            };

            if (responseId != 10)
            {
                mock.Setup(m => m.Fetch()).Returns(data);
            }
            else
            {
                var issue = new Faker().System.Exception;
                mock.Setup(m => m.Fetch()).Throws(issue);
            }
            service.SetIndex(index);
            _ = await service.InstallAsync();
            mock.Verify(m => m.Fetch());
        }

        [Fact]
        public void DentonServiceHasExpectedIndex()
        {
            var service = new MockDentonReader(GetConfiguration());
            Assert.Equal(1, service.ExternalId);
        }

        [Fact]
        public void HarrisServiceHasExpectedIndex()
        {
            var service = new MockHarrisReader(GetConfiguration());
            Assert.Equal(2, service.ExternalId);
        }

        [Fact]
        public void HarrisJpServiceHasExpectedIndex()
        {
            var service = new MockHarrisJpReader(GetConfiguration());
            Assert.Equal(4, service.ExternalId);
        }

        [Fact]
        public void TarrantServiceHasExpectedIndex()
        {
            var service = new MockTarrantReader(GetConfiguration());
            Assert.Equal(3, service.ExternalId);
        }

        private sealed class MockPageReader(bool usingMock = false) : WebVerifyPageReadCollin(GetConfiguration())
        {
            private readonly bool isMockEnabled = usingMock;
            public Mock<WebInteractive> Mock { get; private set; } = new();
            private int _webIndex = 0;
            public int ExternalId => _webIndex;
            protected override int WebId => _webIndex;
            public void SetIndex(int index) { _webIndex = index; }
            public WebInteractive? GetWebInteraction(int id)
            {
                return base.GetWeb(id);
            }
            protected override WebInteractive? GetWeb(int index)
            {
                if (isMockEnabled) return Mock.Object;
                return base.GetWeb(index);
            }
        }

        private sealed class MockDentonReader(IConfiguration configuration) : WebVerifyPageReadDenton(configuration)
        {
            public int ExternalId => WebId;
        }
        private sealed class MockHarrisReader(IConfiguration configuration) : WebVerifyPageReadHarris(configuration)
        {
            public int ExternalId => WebId;
        }
        private sealed class MockHarrisJpReader(IConfiguration configuration) : WebVerifyPageReadHarrisJp(configuration)
        {
            public int ExternalId => WebId;
        }
        private sealed class MockTarrantReader(IConfiguration configuration) : WebVerifyPageReadTarrant(configuration)
        {
            public int ExternalId => WebId;
        }


        private static IConfiguration GetConfiguration()
        {
            return SettingsProvider.GetConfiguration();
        }
    }
}
