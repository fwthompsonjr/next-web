namespace next.core.entities
{
    internal class ContactPhone
    {
        public string PhoneType { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public ContactProfileItem ToItem()
        {
            return new ContactProfileItem
            {
                Category = GetType().Name.Replace("Contact", ""),
                Code = PhoneType,
                Data = Phone,
            };
        }
    }
}