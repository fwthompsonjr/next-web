using next.processor.api.models;

namespace next.processor.api.utility
{
    internal static class PostAddressProvider
    {
        public static string? BaseApiAddress()
        {
            const char comma = ',';
            const string source = "api.permissions:{0}";
            var keys = "destination,remote,local".Split(comma).ToList();
            var values = new List<string?>();
            var config = SettingsProvider.Configuration;
            keys.ForEach(key =>
            {
                var setting = string.Format(source, key);
                var value = config[setting];
                values.Add(value);
            });
            var destination = values[0];
            if (string.IsNullOrEmpty(destination)) return null;
            var id = destination.Equals("local") ? 1 : 0;
            return values[id];
        }


        public static List<ApiAddress>? PostAddresses()
        {
            const char comma = ',';
            const string source = "post_address:{0}";
            var baseapi = BaseApiAddress();
            if (string.IsNullOrEmpty(baseapi)) return null;
            var keys = "initialize,update,fetch,start,status,finalize".Split(comma).ToList();
            var values = new List<ApiAddress>();
            var config = SettingsProvider.Configuration;
            keys.ForEach(key =>
            {
                var setting = string.Format(source, key);
                var value = config[setting];
                if (!string.IsNullOrEmpty(value))
                {
                    var address = $"{baseapi}{value}";
                    values.Add(new() { Name = key, Address = address });
                }
            });
            return values;
        }
    }
}
