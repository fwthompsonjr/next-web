using next.processor.api.extensions;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class BaseQueueRequestTests
    {
        [Theory]
        [InlineData(typeof(QueueCompletionRequest))]
        [InlineData(typeof(QueuedRecord))]
        [InlineData(typeof(QueueInitializeRequest))]
        [InlineData(typeof(QueueRecordStatusRequest))]
        [InlineData(typeof(QueueUpdateRequest))]
        public void TargetHasCorrectBase(Type type)
        {
            var instance = Activator.CreateInstance(type);
            Assert.NotNull(instance);
            Assert.IsAssignableFrom<BaseQueueRequest>(instance);
            if (instance is not BaseQueueRequest request) return;
            Assert.Equal("", request.Source);
            request.AppendSource();
            Assert.NotEmpty(request.Source);
        }
    }
}
