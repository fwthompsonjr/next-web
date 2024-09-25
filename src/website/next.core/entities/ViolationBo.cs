namespace next.core.entities
{
    public class ViolationBo
    {
        public string IpAddress { get; set; } = string.Empty;
        public int Count { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpiryDate => CreateDate.Add(TimeSpan.FromMinutes(30));
    }
}
