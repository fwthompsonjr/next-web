using next.processor.api.models;

namespace next.processor.api.utility
{
    public static class BatchNameProvider
    {
        public static List<BatchDescriptor>? BatchSequence()
        {
            lock (locker)
            {
                if (_batchDescriptors != null) return _batchDescriptors;
                const char comma = ',';
                const string source = "queue_process:{0}";
                var keys = "fetch,process,complete".Split(comma).ToList();
                var values = new List<BatchDescriptor>();
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
                _batchDescriptors = values;
                return _batchDescriptors;
            }
        }

        private static List<BatchDescriptor>? _batchDescriptors;
        private static readonly object locker = new();
    }
}