namespace next.core.interfaces
{
    internal interface IHistoryPersistence
    {
        void Clear();
        void Save(string json);
        string? Fetch();
        void SaveRestriction(string json);
        string? Restriction();
        void SaveFilter(string json);
        string? Filter();
    }
}