using legallead.models.Search;
using legallead.permissions.api.Model;
using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.utility;

namespace next.processor.api.backing
{
    public abstract class BaseQueueProcess(IApiWrapper wrapper) : IQueueProcess
    {
        protected readonly IApiWrapper apiWrapper = wrapper;
        private bool disposedValue;

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
            var obj = record.Payload.ToInstance<SearchRequest>();
            if (obj != null) return obj;
            var usr = record.Payload.ToInstance<UserSearchRequest>();
            if (usr == null) return null;
            return QueueMapper.MapFrom<UserSearchRequest, SearchRequest>(usr);
        }

        protected async Task PostStatusAsync(QueuedRecord? record, int messageId, int statusId)
        {
            try
            {
                if (record == null) return;
                await apiWrapper.PostStatusAsync(record, messageId, statusId);
            }
            catch (Exception)
            {
                // do not report expections
            }
        }

        protected async Task ReportIssueAsync(QueuedRecord? record, Exception exception)
        {
            try
            {
                if (record == null) return;
                await apiWrapper.ReportIssueAsync(record, exception);
            }
            catch (Exception)
            {
                // do not report expections
            }
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

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // no managed objects are used in base class
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code.
            // Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
