using legallead.jdbc.entities;
using Newtonsoft.Json;
using next.processor.api.models;
using next.processor.api.utility;
using System.Text;

namespace next.processor.api.extensions
{
    public static class QueueRequestExtensions
    {
        public static bool IsValid(this QueueInitializeRequest request)
        {
            if (request == null) return false;
            if (string.IsNullOrEmpty(request.MachineName)) return false;
            if (string.IsNullOrEmpty(request.Message)) return false;
            if (!request.StatusId.HasValue) return false;
            if (request.MachineName.Length > 256) return false;
            if (request.Message.Length > 255) return false;
            if (!statuses.Contains(request.StatusId.Value)) return false;
            if (request.Items.Count == 0) return false;
            return true;
        }

        public static bool IsValid(this QueueUpdateRequest request)
        {
            if (request == null) return false;
            if (string.IsNullOrEmpty(request.Id)) return false;
            if (string.IsNullOrEmpty(request.SearchId)) return false;
            if (string.IsNullOrEmpty(request.Message)) return false;
            if (request.Message.Length > 255) return false;
            if (!request.StatusId.HasValue) return false;
            if (!statuses.Contains(request.StatusId.Value)) return false;
            if (!Guid.TryParse(request.Id, out var _)) return false;
            if (!Guid.TryParse(request.SearchId, out var _)) return false;
            if (!statuses.Contains(request.StatusId.Value)) return false;
            return true;
        }

        public static bool IsValid(this QueueRecordStatusRequest request)
        {
            if (request == null) return false;
            if (string.IsNullOrEmpty(request.UniqueId)) return false;
            if (!request.MessageId.HasValue) return false;
            if (!request.StatusId.HasValue) return false;
            return true;
        }

        public static string Serialize(this QueueInitializeRequest request)
        {
            var fallback = new QueueInitializeRequest { StatusId = -100 };
            if (!request.IsValid())
            {
                return JsonConvert.SerializeObject(fallback);
            }
            request.Items.RemoveAll(x =>
            {
                if (string.IsNullOrEmpty(x.Id)) return false;
                return Guid.TryParse(x.Id, out var _);
            });
            return JsonConvert.SerializeObject(request);
        }

        public static void AppendSource(this BaseQueueRequest request)
        {
            if (AppNames.Count == 0) return;
            request.Source = AppNames[0];
        }

        public static void Log(this QueueReportIssueRequest request)
        {
            const string name = Constants.ErrorLogName;
            var expiration = TimeSpan.FromDays(7);
            var item = new TrackErrorModel { Data = request };
            TrackEventService.AppendItem(name, item, expiration);
        }
        public static void Log(this Exception exception, string source = "")
        {
            var message = string.IsNullOrEmpty(source) ? exception.Message : $"{source} : {exception.Message}";
            Console.WriteLine("Error. Message: {0}", message);
            var data = Encoding.UTF8.GetBytes(exception.ToString());
            var issue = new QueueReportIssueRequest
            {
                Id = Guid.NewGuid().ToString(),
                Message = message,
                CreateDate = DateTime.UtcNow,
                Data = data
            };
            issue.Log();
        }

        public static T? ToInstance<T>(this string? json)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(json)) return default;
                var model = JsonConvert.DeserializeObject<T>(json);
                return model;
            }
            catch
            {
                return default;
            }
        }


        public static string ToJsonString(this object? obj)
        {
            try
            {
                if (obj == null) return string.Empty;
                var model = JsonConvert.SerializeObject(obj);
                return model;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static QueueWorkingBo ConvertFrom(this QueueUpdateRequest request)
        {
            return new()
            {
                Id = request.Id,
                SearchId = request.SearchId,
                Message = request.Message,
                StatusId = request.StatusId
            };
        }
        private static readonly int[] statuses = [-1, 0, 1, 2];

        private static List<string> AppNames => appNames ??= GetAppNames();
        private static List<string>? appNames;
        private static List<string> GetAppNames()
        {
            var config = SettingsProvider.Configuration;
            var name = config["api.source"];
            if (string.IsNullOrWhiteSpace(name)) return [];
            return [name];
        }
    }

}
