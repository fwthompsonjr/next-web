using legallead.records.search.Classes;
using legallead.records.search.Models;
using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.models;
using next.processor.api.services;

namespace next.processor.api.backing
{
    public class QueueProcessSearch(
        IApiWrapper wrapper,
        IExcelGenerator excelservice,
        IWebInteractiveWrapper? webwrapper = null) : BaseQueueProcess(wrapper)
    {
        private readonly IExcelGenerator _generator = excelservice;
        private readonly IWebInteractiveWrapper _web = webwrapper ?? new WebInteractiveWrapper();
        public override int Index => MessageIndexes.RequestProcessing;

        public override string Name => "Process Read Records Request";

        public override bool IsSuccess { get; protected set; }
        public override bool AllowIterateNext { get; protected set; }

        public override async Task<QueueProcessResponses?> ExecuteAsync(QueueProcessResponses? record)
        {
            IsSuccess = false;
            AllowIterateNext = false;
            var processIndex = Index;
            var item = record?.QueuedRecord;
            var interaction = record?.WebReader;
            if (record == null || item == null || interaction == null)
            {
                IsSuccess = false;
                AllowIterateNext = false;
                return record;
            }
            try
            {
                // set timestamp for step started
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Begin);
                var response = Fetch(interaction);
                if (response == null) return record;
                record.FetchResult = response;
                var people = response.PeopleList.ToJsonString().ToInstance<List<QueuePersonItem>>();
                if (people == null) return record;
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Complete);
                // iterate next step name
                processIndex = MessageIndexes.TranslateRecords;
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Begin);
                var addresses = _generator.GetAddresses(response);
                if (addresses == null) return record;
                record.Addresses = addresses;
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Complete);
                // iterate next step name
                processIndex = MessageIndexes.SerializeRecords;
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Begin);
                var serialized = _generator.SerializeResult(addresses);
                if (serialized.Length == 0) return record;
                await apiWrapper.PostSaveContentAsync(item, serialized);
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Complete);
                // iterate next step name
                processIndex = MessageIndexes.CompleteProcess;
                await apiWrapper.PostStatusAsync(item, processIndex, StatusIndexes.Begin);
                await apiWrapper.PostStepFinalizedAsync(item, people);

                IsSuccess = true;
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

        public WebFetchResult? Fetch(WebInteractive web)
        {
            return _web.Fetch(web);
        }
    }
}