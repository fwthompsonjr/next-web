using Newtonsoft.Json;

namespace next.web.core.models
{
    internal class FormRegistrationModel
    {
        [JsonProperty("username")]
        public string UserName { get; set; } = string.Empty;
        [JsonProperty("register-password")]
        public string Password { get; set; } = string.Empty;
        [JsonProperty("register-email")]
        public string Email { get; set; } = string.Empty;
        [JsonProperty("register-password-confirmation")]
        public string PasswordConfirmation { get; set; } = string.Empty;
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(UserName)) return false;
                if (string.IsNullOrEmpty(Password)) return false;
                if (string.IsNullOrEmpty(Email)) return false;
                if (string.IsNullOrEmpty(PasswordConfirmation)) return false;
                return Password.Equals(PasswordConfirmation);
            }
        }
    }
}