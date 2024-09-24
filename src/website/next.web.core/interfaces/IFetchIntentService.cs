namespace next.web.core.interfaces
{
    public interface IFetchIntentService
    {
        Task<string?> GetIntent(string url, string request);
    }
}
