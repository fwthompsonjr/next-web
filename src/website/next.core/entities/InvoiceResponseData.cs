using Newtonsoft.Json;

namespace next.core.entities
{
    public class InvoiceResponseData
    {
        [JsonProperty("lineId")]
        public string? LineId { get; set; }

        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("itemType")]
        public string? ItemType { get; set; }

        [JsonProperty("itemCount")]
        public int? ItemCount { get; set; }

        [JsonProperty("unitPrice")]
        public double? UnitPrice { get; set; }

        [JsonProperty("price")]
        public double? Price { get; set; }

        [JsonProperty("referenceId")]
        public string? ReferenceId { get; set; }

        [JsonProperty("externalId")]
        public string? ExternalId { get; set; }

        [JsonProperty("purchaseDate")]
        public DateTime? PurchaseDate { get; set; }

        [JsonProperty("isDeleted")]
        public bool? IsDeleted { get; set; }

        [JsonProperty("createDate")]
        public DateTime? CreateDate { get; set; }
    }
}
