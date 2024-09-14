namespace next.core.entities
{
    internal class ContactUsStateResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public string UiControlId
        {
            get
            {
                const string controlFormat = "permissions-discounts-{0}";
                if (string.IsNullOrEmpty(ShortName)) return string.Empty;
                var st = ShortName.ToLower().Trim();
                return string.Format(controlFormat, st);
            }
        }
    }
}