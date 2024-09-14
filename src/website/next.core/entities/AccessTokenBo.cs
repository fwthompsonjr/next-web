using Newtonsoft.Json;

namespace next.core.entities
{
    internal class AccessTokenBo
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("refreshToken")]
        public string RefreshToken { get; set; } = string.Empty;

        [JsonProperty("expires")]
        public DateTime? Expires { get; set; }
    }
}