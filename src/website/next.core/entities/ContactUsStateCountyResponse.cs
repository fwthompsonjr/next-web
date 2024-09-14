using System.Text;

namespace next.core.entities
{
    internal class ContactUsStateCountyResponse
    {
        public string Name { get; set; } = string.Empty;
        public int Index { get; set; }
        public string StateCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string IndexCode => Index.ToString();

        public string UiControlId
        {
            get
            {
                const string controlFormat = "permissions-discounts-{0}-{1}";
                if (string.IsNullOrEmpty(Name)) return string.Empty;
                if (string.IsNullOrEmpty(StateCode)) return string.Empty;
                var st = StateCode.ToLower().Trim();
                var name = new StringBuilder(Name.ToLower().Trim());
                name.Replace(' ', '-');
                return string.Format(controlFormat, st, name);
            }
        }
    }
}