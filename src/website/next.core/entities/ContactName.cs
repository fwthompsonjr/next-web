namespace next.core.entities
{
    internal class ContactName
    {
        public string NameType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public ContactProfileItem ToItem()
        {
            return new ContactProfileItem
            {
                Category = GetType().Name.Replace("Contact", ""),
                Code = NameType,
                Data = Name,
            };
        }
    }
}