using Newtonsoft.Json;

namespace next.core.entities
{
    internal class UserSearchFilterBo
    {
        [JsonProperty("index")]
        public int Index { get; set; }
        [JsonProperty("county")]
        public string County { get; set; } = string.Empty;
        public bool HasFilter
        {
            get
            {
                if (Index != 0) return true;
                if (!string.IsNullOrEmpty(County)) return true;
                return false;
            }
        }

        public string GetCaption()
        {
            const string fallback = "None";
            if (!HasFilter) return fallback;
            var message = fallback;
            var statusName = Index switch
            {
                10 => "Error",
                1 => "Submitted",
                2 => "Processing",
                3 => "Completed",
                4 => "Purchased",
                5 => "Downloaded",
                _ => string.Empty
            };
            if (Index != 0 && !string.IsNullOrEmpty(statusName))
            {
                message = $"Status: {statusName}";
            }
            if (string.IsNullOrEmpty(County)) return message;
            if (message.Equals(fallback))
            {
                message = $"County: {County}";
                return message;
            }
            message += $", County: {County}";
            return message;
        }
    }
}
