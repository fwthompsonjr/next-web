using Bogus;
using legallead.records.search.Classes;
using legallead.records.search.Models;
using Moq;
using next.processor.api.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [InlineData(4, false)]
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
        [InlineData(4)]
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
            var service = new MockDentonReader();
            Assert.Equal(1, service.ExternalId);
        }

        [Fact]
        public void HarrisServiceHasExpectedIndex()
        {
            var service = new MockHarrisReader();
            Assert.Equal(2, service.ExternalId);
        }

        [Fact]
        public void TarrantServiceHasExpectedIndex()
        {
            var service = new MockTarrantReader();
            Assert.Equal(3, service.ExternalId);
        }

        private sealed class MockPageReader(bool usingMock = false) : WebVerifyPageReadCollin
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

        private sealed class MockDentonReader : WebVerifyPageReadDenton
        {
            public int ExternalId => WebId;
        }
        private sealed class MockHarrisReader : WebVerifyPageReadHarris
        {
            public int ExternalId => WebId;
        }
        private sealed class MockTarrantReader : WebVerifyPageReadTarrant
        {
            public int ExternalId => WebId;
        }
    }
}
