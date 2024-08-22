namespace next.processor.api.tests.backing
{
    public class BaseTimedServiceTest
    {
        [Theory]
        [InlineData(true, 15, 5)]
        [InlineData(false, 15, 5)]
        [InlineData(false, 15, 5, true)]
        public void SeriveCanBeConstructed(bool b, int i, int j, bool isnull = false)
        {
            var error = Record.Exception(() =>
            {
                using var service = new MockTimedService(b, i, j, isnull);
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, 15, 5)]
        [InlineData(false, 15, 5)]
        public async Task SeriveCanBeStartedAsync(bool b, int i, int j)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var service = new MockTimedService(b, i, j);
                await service.Execute();
            });
            Assert.Null(error);
        }


        [Theory]
        [InlineData(true, 15, 5)]
        [InlineData(false, 15, 5)]
        public async Task SeriveCanBeStartedAndStoppedAsync(bool b, int i, int j)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var service = new MockTimedService(b, i, j);
                await service.Execute();
                await service.Stop();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, 15, 5)]
        [InlineData(false, 15, 5)]
        public void SeriveCanRunTimer(bool b, int i, int j)
        {
            var error = Record.Exception(() =>
            {
                using var service = new MockTimedService(b, i, j);
                service.RunTimer();
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(true, 15, 5)]
        [InlineData(false, 15, 5)]
        public async Task SeriveCanBeStoppedAsync(bool b, int i, int j)
        {
            var error = await Record.ExceptionAsync(async () =>
            {
                using var service = new MockTimedService(b, i, j);
                await service.Stop();
            });
            Assert.Null(error);
        }
    }
}
