using next.processor.api.extensions;
using next.processor.api.interfaces;
using next.processor.api.models;

namespace next.processor.api
{
    internal static class TrackEventService
    {
        public static List<TrackEventModel> Models => _models;

        public static void Clear()
        {
            lock (_locker) { Models.Clear(); }
        }
        public static void Expire()
        {
            lock (_locker)
            {
                var currentDt = DateTime.UtcNow;
                var found = Models.Count(x => x.ExpirationDate <= currentDt);
                if (found == 0) return;
                Models.RemoveAll(x => x.ExpirationDate <= currentDt);
            }
        }


        public static bool Exists(string keyName)
        {
            lock (_locker)
            {
                var comparison = StringComparer.OrdinalIgnoreCase;
                var collection = Models.Select(x => x.Name).ToList();
                return collection.Contains(keyName, comparison);
            }
        }
        public static void AddOrUpdate(string keyName, string keyValue, TimeSpan expiration)
        {
            lock (_locker)
            {
                const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                var found = Models.Find(x => x.Name.Equals(keyName, comparison));
                var expirationDate = DateTime.UtcNow.Add(expiration);
                if (found != null)
                {
                    found.Content = keyValue;
                    found.ExpirationDate = expirationDate;
                    return;
                }
                found = new TrackEventModel
                {
                    Content = keyValue,
                    ExpirationDate = expirationDate,
                    Name = keyName
                };
                Models.Add(found);
            }
        }

        public static void AppendItem<T>(string keyName, T item, TimeSpan expiration) where T : ITrackable
        {
            lock (_locker)
            {
                const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                var expirationDate = DateTime.UtcNow.Add(expiration);
                item.ExpirationDate = expirationDate;
                var found = Models.Find(x => x.Name.Equals(keyName, comparison));
                if (found != null)
                {
                    found.ExpirationDate = expirationDate;
                    AppendOrUpdateItem(found, item);
                    return;
                }
                found = new TrackEventModel
                {
                    Content = new List<T> { item }.ToJsonString(),
                    ExpirationDate = expirationDate,
                    Name = keyName
                };
                Models.Add(found);
            }
        }
        public static string? Get(string keyName)
        {
            lock (_locker)
            {
                const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
                var found = Models.Find(x => x.Name.Equals(keyName, comparison));
                return found?.Content ?? null;
            }
        }
        private static void AppendOrUpdateItem<T>(TrackEventModel parent, T item) where T : ITrackable
        {
            var list = parent.Content.ToInstance<List<T>>();
            if (list == null) return;
            var found = list.FindIndex(x => x.Id.Equals(item.Id));
            if (found >= 0)
            {
                list[found] = item;
            }
            else
            {
                list.Add(item);
            }
            var currentDt = DateTime.UtcNow;
            list.RemoveAll(x => x.ExpirationDate <= currentDt);
            parent.Content = list.ToJsonString();
        }



        private static readonly List<TrackEventModel> _models = new();
        private static readonly object _locker = new();
    }
}