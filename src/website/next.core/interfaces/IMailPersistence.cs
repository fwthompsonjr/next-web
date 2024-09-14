namespace next.core.interfaces
{
    internal interface IMailPersistence
    {
        void Clear();
        void Save(string json);
        void Save(string id, string json);
        string? Fetch();
        string? Fetch(string id);
        bool DoesItemExist(string id);
    }
}
