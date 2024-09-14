namespace next.core.entities
{
    internal interface ISearchIndexable
    {
        string this[int index] { get; }
    }
}