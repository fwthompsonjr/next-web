using legallead.models.Search;
using legallead.permissions.api.Model;
using next.processor.api.backing;
using next.processor.api.models;

namespace next.processor.api.tests
{
    internal class MockQueueProcess : BaseQueueProcess
    {
        public MockQueueProcess() : base(GetWrapper())
        {

        }

        public void WriteSuccess(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }
        public void WriteIterateNext(bool isAllowed)
        {
            AllowIterateNext = isAllowed;
        }

        public UserSearchRequest? TranslateToUserSearchRequest(QueuedRecord? record)
        {
            return GetUserSearchRequest(record);
        }

        public SearchRequest? TranslateToSearchRequest(QueuedRecord? record)
        {
            return GetSearchRequest(record);
        }
        public override int Index => 1000;

        public override string Name => "Test implementation of queue process";

        public override bool IsSuccess { get; protected set; }
        public override bool AllowIterateNext { get; protected set; }

        private static MockApiWrapperService GetWrapper()
        {
            return new MockApiWrapperService();
        }

        public override Task<QueueProcessResponses?> ExecuteAsync(QueueProcessResponses? record)
        {
            return Task.FromResult(record);
        }
    }
}
