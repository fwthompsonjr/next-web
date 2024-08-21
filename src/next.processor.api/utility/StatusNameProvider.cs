using next.processor.api.models;

namespace next.processor.api.utility
{
    internal static class StatusNameProvider
    {
        public static List<ItemDescriptor>? StatusSequence()
        {
            lock (locker)
            {
                if (_statuses != null) return _statuses;
                const char comma = ',';
                const string source = "status_names:{0}";
                var keys = "-1,0,1,2".Split(comma).ToList();
                var values = new List<ItemDescriptor>();
                var config = SettingsProvider.Configuration;
                keys.ForEach(key =>
                {
                    var setting = string.Format(source, key);
                    var value = config[setting];
                    if (!string.IsNullOrEmpty(value))
                    {
                        values.Add(new()
                        {
                            Id = keys.IndexOf(key),
                            Name = key,
                            Descriptor = value
                        });
                    }
                });
                _statuses = values;
                return _statuses;
            }
        }

        private static List<ItemDescriptor>? _statuses;
        private static readonly object locker = new();
    }
}