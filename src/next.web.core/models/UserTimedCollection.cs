namespace next.web.core.models
{
    internal class UserTimedCollection<T>(T collection, TimeSpan expiry)
    {
        public DateTime ExpirationDate { get; } = DateTime.UtcNow.Add(expiry);
        public T Value { get; } = collection;
        public bool IsExpired => ExpirationDate < DateTime.UtcNow;
    }
}
