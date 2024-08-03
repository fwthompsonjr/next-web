using Newtonsoft.Json;
using next.web.core.extensions;
using System.Diagnostics;

namespace next.web.core.models
{
    internal class UserTimedCollection<T>
    {
        public UserTimedCollection() { }
        internal UserTimedCollection(T collection, TimeSpan expiry)
        {
            var dtenow = DateTime.UtcNow;
            var expireDt = dtenow.Add(expiry);
            var json = JsonConvert.SerializeObject(collection);
            ContentJs = json;
            ExpirationDate = expireDt;
            Debug.WriteLine("Current Date: {0:f}", dtenow);
            Debug.WriteLine("Future Date: {0:f}", expireDt);
            Debug.WriteLine("Is expired: {0}", IsExpired());
            Debug.WriteLine("");
        }

        public DateTime ExpirationDate { get; set; }
        public string ContentJs { get; set; } = string.Empty;
        public T? GetValue()
        {
            if (ContentJs == null) return default;
            return ContentJs.ToInstance<T>();
        }
        public bool IsExpired()
        {
            var dtenow = DateTime.UtcNow;
            var seconds = dtenow.Subtract(ExpirationDate).TotalSeconds;
            var isExpired = seconds > 0;
            return isExpired;
        }
    }
}
