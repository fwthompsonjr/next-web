namespace next.web.Models
{
    public class DownloadJsResponse
    {
        public string? ExternalId { get; set; }
        public string? Description { get; set; }
        public string? Content { get; set; }
        public string? Error { get; set; }
        public string? CreateDate { get; set; }

        public string FileName()
        {
            var fallback = $"{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.xlsx";
            List<string> expected = [ ":", "-", " to ", " on" ];
            if (string.IsNullOrEmpty(Description)) return fallback;
            var missing = expected.Count(x => !Description.Contains(x));
            if (missing > 0) return fallback;
            var a = Description.IndexOf(" on ");
            var line = Description.Substring(0, a).Split(':')[^1].Replace(" to ", "-").Replace(" ", "").ToUpper();
            return $"{line}.xlsx";
        }
    }
}
