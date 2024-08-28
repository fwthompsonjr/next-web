using Moq;
using next.processor.api.backing;
using next.processor.api.interfaces;

namespace next.processor.api.tests
{
    internal class MockSearchGenerationService() : SearchGenerationService(GetExecutor())
    {

        public void Work()
        {
            DoWork(null);
        }

        public string EchoMyHealth()
        {
            return GetHealth();
        }
        public string EchoMyStatus()
        {
            return GetStatus();
        }

        private static IQueueExecutor GetExecutor()
        {
            var mock = new Mock<IQueueExecutor>();
            return mock.Object;
        }
    }
}
