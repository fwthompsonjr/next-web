using legallead.models.Search;
using legallead.permissions.api.Model;
using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.models;

namespace next.processor.api.backing
{
    public abstract class BaseQueueProcess(IApiWrapper wrapper) : IQueueProcess
    {
        protected readonly IApiWrapper apiWrapper = wrapper;

        public abstract int Index { get; }
        public abstract string Name { get; }
        public abstract bool IsSuccess { get; protected set; }
        public abstract bool AllowIterateNext { get; protected set; }
        public abstract Task<QueueProcessResponses?> ExecuteAsync(QueueProcessResponses? record);

        protected UserSearchRequest? GetUserSearchRequest(QueuedRecord? record)
        {
            if (record == null || string.IsNullOrEmpty(record.Payload)) return null;
            return record.Payload.ToInstance<UserSearchRequest>();
        }

        protected SearchRequest? GetSearchRequest(QueuedRecord? record)
        {
            if (record == null || string.IsNullOrEmpty(record.Payload)) return null;
            return record.Payload.ToInstance<SearchRequest>();
        }

        protected static class MessageIndexes
        {
            public const int BeginProcess = 0;
            public const int ParameterEvaluation = 1;
            public const int ParameterConversion = 2;
            public const int RequestProcessing = 3;
            public const int TranslateRecords = 4;
            public const int SerializeRecords = 5;
            public const int CompleteProcess = 6;
        }

        protected static class StatusIndexes
        {
            public const int Begin = 0;
            public const int Complete = 1;
            public const int Failed = 2;
        }
    }
}
