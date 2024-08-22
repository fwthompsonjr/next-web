using next.processor.api.models;

namespace next.processor.api.utility
{
    internal static class ItemNameProvider
    {
        public static List<ItemDescriptor>? ItemSequence()
        {
            lock (locker)
            {
                if (_itemDescriptors != null) return _itemDescriptors;
                const char comma = ',';
                const string source = "item_process:{0}";
                var keys = "start,get_parameter,convert_parameter,execute_search,translate_excel,serialize".Split(comma).ToList();
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
                _itemDescriptors = values;
                return _itemDescriptors;
            }
        }

        private static List<ItemDescriptor>? _itemDescriptors;
        private static readonly object locker = new();
    }
}
