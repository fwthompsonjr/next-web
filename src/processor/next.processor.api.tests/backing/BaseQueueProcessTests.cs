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
    }
}
