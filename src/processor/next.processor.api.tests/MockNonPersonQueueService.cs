using Bogus;
using legallead.jdbc.entities;
using Moq;
using next.processor.api.backing;
using next.processor.api.interfaces;
using System.Text;

namespace next.processor.api.tests
{
    internal class MockNonPersonQueueService() : NonPersonQueueService(GetExecutor())
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

        private static IApiWrapper GetExecutor()
        {
            var faker = new Faker();
            var mock = new Mock<IApiWrapper>();
            var list = new List<QueueNonPersonBo>
            {
                new() {
                    Id = faker.Random.Guid().ToString(),
                    ExcelData = Encoding.UTF8.GetBytes(faker.Hacker.Phrase()) },
                new() {
                    Id = faker.Random.Guid().ToString(),
                    ExcelData = Encoding.UTF8.GetBytes(faker.Hacker.Phrase()) }
            };
            mock.Setup(m => m.FetchNonPersonAsync()).ReturnsAsync(list);
            mock.Setup(m => m.PostSaveNonPersonAsync(It.IsAny<QueueNonPersonBo>())).Verifiable();
            return mock.Object;
        }
    }
}