namespace next.web.core.models
{
    public class FormSubmissionModel
    {
        public string? FormName { get; set; }
        public string? Payload { get; set; }
        public bool IsValid
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FormName)) return false;
                if (string.IsNullOrWhiteSpace(Payload)) return false;
                return true;
            }
        }
    }
}
