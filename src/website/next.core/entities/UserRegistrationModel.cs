using Newtonsoft.Json;

namespace next.core.entities
{
    internal class UserRegistrationModel
    {
        [JsonProperty("username")]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("register-password")]
        public string Password { get; set; } = string.Empty;

        [JsonProperty("register-email")]
        public string Email { get; set; } = string.Empty;

        public object ToApiModel()
        {
            return new { UserName, Password, Email };
        }
    }
}