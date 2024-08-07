using Newtonsoft.Json;

namespace next.web.core.models
{
    internal class UserIdentityBo
    {
        [JsonProperty("userName", NullValueHandling = NullValueHandling.Ignore)]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("email", NullValueHandling = NullValueHandling.Ignore)]
        public string Email { get; set; } = string.Empty;

        [JsonProperty("created", NullValueHandling = NullValueHandling.Ignore)]
        public string Created { get; set; } = string.Empty;

        [JsonProperty("role", NullValueHandling = NullValueHandling.Ignore)]
        public string Role { get; set; } = string.Empty;

        [JsonProperty("roleDescription", NullValueHandling = NullValueHandling.Ignore)]
        public string RoleDescription { get; set; } = string.Empty;

        [JsonProperty("fullName", NullValueHandling = NullValueHandling.Ignore)]
        public string FullName { get; set; } = string.Empty;
    }
}