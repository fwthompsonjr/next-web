namespace next.web.core.models
{
    internal class UserContextBo
    {

        public string UserName { get; set; } = string.Empty;
        public string AppName { get; set; } = string.Empty;
        public string AppId { get; set; } = string.Empty;

        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? Expires { get; set; }

    }
}
