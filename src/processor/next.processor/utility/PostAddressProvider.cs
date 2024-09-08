using next.processor.api.models;

namespace next.processor.api.utility
{
    public static class PostAddressProvider
    {
        public static string? BaseApiAddress()
        {
            lock (locker)
            {
                if (!string.IsNullOrWhiteSpace(_baseapi)) return _baseapi;
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
                var id = destination.Equals("local") ? 2 : 1;
                _baseapi = values[id];
                return _baseapi;
            }
        }


        public static List<ApiAddress>? PostAddresses()
        {
            lock (locker)
            {
                if (_postaddresses != null) return _postaddresses;
                const char comma = ',';
                const string source = "post_address:{0}";
                var baseapi = BaseApiAddress();
                if (string.IsNullOrEmpty(baseapi)) return null;
                var keys = "initialize,update,fetch,start,status,complete,finalize,save,queue-status,queue-summary".Split(comma).ToList();
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
                _postaddresses = values;
                return _postaddresses;
            }
        }

        private static string? _baseapi;
        private static List<ApiAddress>? _postaddresses;
        private static readonly object locker = new();
    }
}
