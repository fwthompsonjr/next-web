namespace next.core.entities
{
    internal class MyPurchaseBo
    {
        public DateTime? PurchaseDate { get; set; }
        public string? ReferenceId { get; set; }
        public string? ExternalId { get; set; }
        public string? ItemType { get; set; }
        public int? ItemCount { get; set; }
        public decimal? Price { get; set; }
        public string? StatusText { get; set; }
    }
}
