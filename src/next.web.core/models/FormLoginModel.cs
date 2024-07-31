using Newtonsoft.Json;

namespace next.web.core.models
{
    internal class FormLoginModel
    {
        [JsonProperty("username")]
        public string UserName { get; set; } = string.Empty;
        [JsonProperty("login-password")]
        public string Password { get; set; } = string.Empty;
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(UserName)) return false;
                if (string.IsNullOrEmpty(Password)) return false;
                return true;
            }
        }
    }
}
