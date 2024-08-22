using legallead.models.Search;
using legallead.permissions.api.Model;
using legallead.records.search.Classes;
using legallead.records.search.Models;
using OfficeOpenXml;

namespace next.processor.api.models
{
    public class QueueProcessResponses(List<QueuedRecord> records)
    {
        public List<QueuedRecord> CurrentBatch { get; } = records;
        public string? UniqueId { get; private set; }
        public QueuedRecord? QueuedRecord { get; private set; }
        public int CurrentIndex { get; private set; } = -1;

        public WebInteractive? WebReader { get; set; }
        public UserSearchRequest? UserRequest { get; set; }

        public SearchRequest? SearchRequest { get; set; }
        public WebFetchResult? FetchResult { get; set; }
        public ExcelPackage? Addresses { get; set; }
        public bool IterateNext()
        {
            UniqueId = null;
            QueuedRecord = null;
            WebReader = null;
            UserRequest = null;
            SearchRequest = null;
            FetchResult = null;
            Addresses = null;
            var id = CurrentIndex + 1;
            if (CurrentBatch == null || id > CurrentBatch.Count - 1) return false;
            CurrentIndex = id;
            QueuedRecord = CurrentBatch[id];
            UniqueId = CurrentBatch[id].Id ?? string.Empty;
            return true;
        }
    }
}
