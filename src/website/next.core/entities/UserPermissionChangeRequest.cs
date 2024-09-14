namespace next.core.entities
{
    internal class UserPermissionChangeRequest
    {
        public string Subscription { get; set; } = string.Empty;
        public string Discounts { get; set; } = string.Empty;
        public string Changes { get; set; } = string.Empty;

        public bool CanSubmit
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Subscription) &&
                    string.IsNullOrWhiteSpace(Discounts) &&
                    string.IsNullOrWhiteSpace(Changes)) return false;
                return true;
            }
        }

        public string SubmissionName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Subscription)) return "Subscription";
                if (!string.IsNullOrWhiteSpace(Discounts)) return "Discounts";
                if (!string.IsNullOrWhiteSpace(Changes)) return "Changes";
                return "";
            }
        }
    }
}