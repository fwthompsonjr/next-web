using Newtonsoft.Json;

namespace next.core.entities
{
    internal class ErrorStatusMessage
    {
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        [JsonProperty("isDefault")]
        public bool? IsDefault { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; } = string.Empty;

        [JsonProperty("description")]
        public string[] Message { get; set; } = Array.Empty<string>();

        public string Description
        {
            get
            {
                if (Message.Length == 0) return string.Empty;
                var msg = string.Join(" ", Message);
                return msg;
            }
        }

        private static List<ErrorStatusMessage>? _messages;

        private static readonly ErrorStatusMessage defaultStatusMessage = new()
        {
            Id = "500",
            IsDefault = true,
            Code = "Unexpected Error",
            Message = new string[]
            {
                "Application encountered an unexpected error. ",
                "Please contact system administrator for additional details or information regarding this error.",
            }
        };

        public static List<ErrorStatusMessage> GetMessages()
        {
            if (_messages != null) return _messages;
            var messages = new List<ErrorStatusMessage>();
            var source = Properties.Resources.errorstatus_json;
            var content = ObjectExtensions.TryGet<List<ErrorStatusMessage>>(source);
            messages.AddRange(content);
            if (!messages.Any()) { messages.Add(defaultStatusMessage); }
            _messages = messages;
            return _messages;
        }
    }
}