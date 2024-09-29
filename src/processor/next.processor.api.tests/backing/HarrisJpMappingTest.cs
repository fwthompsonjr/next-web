using legallead.records.search.Classes;
using Moq;
using next.processor.api.backing;
using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.models;
using System.Diagnostics;

namespace next.processor.api.tests.backing
{
    public class HarrisJpMappingTest
    {
        [Fact]
        public async Task MappingTestAsync()
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                var mapped = await GetResponsesAsync();
                Assert.NotNull(mapped);
                Assert.IsAssignableFrom<HarrisJpInteractive>(mapped.WebReader);
            });
            Assert.Null(error);
        }
        [Fact]
        public async Task HarrisSearchTestAsync()
        {
            if (!Debugger.IsAttached) return;
            var error = await Record.ExceptionAsync(async () =>
            {
                var mapped = await GetResponsesAsync();
                if (mapped?.WebReader is not HarrisJpInteractive interactive) return;
                var result = interactive.Fetch();
                Assert.NotNull(result);
            });
            Assert.Null(error);
        }
        private static async Task<QueueProcessResponses?> GetResponsesAsync()
        {

            var wrapper = GetWrapper();
            var queue = GetQueue();
            if (!queue.IterateNext()) return null;
            var service = new QueueProcessParameter(wrapper);
            var mapped = await service.ExecuteAsync(queue);
            return mapped;
        }
        private static IApiWrapper GetWrapper()
        {
            var mock = new Mock<IApiWrapper>();
            mock.Setup(s => s.StartAsync(It.IsAny<QueuedRecord>())).Returns(Task.CompletedTask);
            mock.Setup(s => s.PostStatusAsync(It.IsAny<QueuedRecord>(), It.IsAny<int>(), It.IsAny<int>())).Returns(Task.CompletedTask);
            return mock.Object;
        }

        private static QueueProcessResponses GetQueue()
        {
            var list = queueData.ToInstance<List<QueuedRecord>>() ?? [];
            return new(list);
        }

        private static readonly string queueData = Properties.Resources.harris_jp_queue_sample_01;
    }
}
