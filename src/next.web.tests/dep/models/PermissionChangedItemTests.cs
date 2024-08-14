using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class PermissionChangedItemTests
    {
        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = MockObjectProvider.GetList<PermissionChangedItem>(2) ?? [];
                Assert.NotEmpty(sut);
                Assert.NotEqual(sut[0].Id, sut[1].Id);
                Assert.NotEqual(sut[0].UserId, sut[1].UserId);
                Assert.NotEqual(sut[0].CustomerId, sut[1].CustomerId);
                Assert.NotEqual(sut[0].ExternalId, sut[1].ExternalId);
                Assert.NotEqual(sut[0].InvoiceUri, sut[1].InvoiceUri);
                Assert.NotEqual(sut[0].LevelName, sut[1].LevelName);
                Assert.NotEqual(sut[0].SessionId, sut[1].SessionId);
                Assert.NotEqual(sut[0].CompletionDate, sut[1].CompletionDate);
                Assert.NotEqual(sut[0].CreateDate, sut[1].CreateDate);
                _ = sut[0].IsPaymentSuccess;
            });
            Assert.Null(error);
        }
    }
}