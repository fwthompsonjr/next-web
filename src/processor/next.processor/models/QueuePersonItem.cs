namespace next.processor.api.models
{
    public class QueuePersonItem
    {
        public string Name { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Address3 { get; set; } = string.Empty;
        public string CaseNumber { get; set; } = string.Empty;
        public string DateFiled { get; set; } = string.Empty;
        public string Court { get; set; } = string.Empty;
        public string CaseType { get; set; } = string.Empty;
        public string CaseStyle { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Plantiff { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        public bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(Zip))
                {
                    return false;
                }

                if (string.IsNullOrEmpty(Address1))
                {
                    return false;
                }

                return true;
            }
        }
    }
}
