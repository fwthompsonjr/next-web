using next.processor.api.models;

namespace next.processor.api.utility
{
    public static class MessageNameProvider
    {
        public static List<ItemDescriptor>? MessageSequence()
        {
            lock (locker)
            {
                if (_messages != null) return _messages;
                const char comma = ',';
                const string source = "message_names:{0}";
                var keys = "0,1,2,3,4,5,6".Split(comma).ToList();
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
                _messages = values;
                return _messages;
            }
        }

        private static List<ItemDescriptor>? _messages;
        private static readonly object locker = new();
    }
}
