using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.utility;

namespace next.processor.api.backing
{
    public class QueueProcessParameter(IApiWrapper wrapper) : BaseQueueProcess(wrapper)
    {
        public override int Index => MessageIndexes.BeginProcess;

        public override string Name => "Map Request";

        public override bool IsSuccess { get; protected set; }
        public override bool AllowIterateNext { get; protected set; }

        public override async Task<QueueProcessResponses?> ExecuteAsync(QueueProcessResponses? record)
        {
            if (record?.QueuedRecord != null)
            {
                await apiWrapper.StartAsync(record.QueuedRecord);
                await apiWrapper.PostStatusAsync(record.QueuedRecord, MessageIndexes.BeginProcess, StatusIndexes.Begin); // notify process begin
            }
            IsSuccess = false;
            var processIndex = MessageIndexes.BeginProcess;
            var item = record?.QueuedRecord;
            if (record == null || item == null || string.IsNullOrEmpty(item.Payload))
            {
                IsSuccess = false;
                AllowIterateNext = false;
                return record;
            }
            try
            {
                
                // set timestamp for step started
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Begin);
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Complete);
                // iterate next step name
                processIndex = MessageIndexes.ParameterEvaluation;
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Begin);
                var user = GetUserSearchRequest(item);
                var search = GetSearchRequest(item);
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Complete);
                // iterate next step name
                processIndex = MessageIndexes.ParameterConversion;
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Begin);
                if (user != null) { 
                    record.UserRequest = user;
                    record.WebReader = QueueMapper.MapFrom<UserSearchRequest, WebInteractive>(user);
                }
                if (search != null) { record.SearchRequest = search; }
                IsSuccess = record.WebReader != null;
                AllowIterateNext = IsSuccess;
                return record;
            } 
            catch (Exception ex)
            {
                // report error details to server
                await apiWrapper.ReportIssueAsync(item, ex);
                return record;
            }
            finally
            {
                var statusIndex = IsSuccess ? StatusIndexes.Complete : StatusIndexes.Failed;
                await apiWrapper.PostStatusAsync(item, processIndex, statusIndex);
            }
        }
    }
}