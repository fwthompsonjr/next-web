namespace next.core.entities
{
    internal class ContactAddress
    {
        public string AddressType { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public ContactProfileItem ToItem()
        {
            return new ContactProfileItem
            {
                Category = GetType().Name.Replace("Contact", ""),
                Code = AddressType,
                Data = Address,
            };
        }
    }
}