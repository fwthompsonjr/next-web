namespace next.web.core.interfaces
{
    public interface ISessionStringWrapper
    {
        string? GetString(string key);
        void SetString(string key, string value);
        void Remove(string key);
    }
}
