using Newtonsoft.Json;
using next.processor.api.models;

namespace next.processor.api.utility
{
    internal static class LocalCountyProvider
    {
        public static LocalCountyItem? FindItem(string countyName, string stateCode = "TX")
        {
            lock (locker)
            {
                var oic = StringComparison.OrdinalIgnoreCase;
                return Items().Find(x => x.Name.Equals(countyName, oic) && x.StateCode.Equals(stateCode, oic));
            }
        }

        public static List<LocalCountyItem> Items()
        {
            lock (locker)
            {
                if (_countyitems != null) return _countyitems;
                var json = Properties.Resources.county_list_json;
                var values = JsonConvert.DeserializeObject<List<LocalCountyItem>>(json) ?? [];
                values.ForEach(x =>
                { 
                    var webid = GetWebIndex(x.Name, x.StateCode);
                    if (webid != null) x.WebId = webid;
                });

                _countyitems = values;
                return _countyitems;
            }
        }
        public static int? GetWebIndex(string? county, string? st)
        {
            if (string.IsNullOrWhiteSpace(county) ||
                string.IsNullOrWhiteSpace(st)) return null;
            var lookup = $"{st.ToLower()}-{county.Replace(' ', '-').ToLower()}";
            if (lookup.Equals("tx-harris")) return 30;
            if (lookup.Equals("tx-collin")) return 20;
            if (lookup.Equals("tx-tarrant")) return 10;
            if (lookup.Equals("tx-denton")) return 0;
            return null;
        }

        private static List<LocalCountyItem>? _countyitems;
        private static readonly object locker = new();
    }
}