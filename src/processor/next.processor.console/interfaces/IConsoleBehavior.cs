namespace next.processor.console.interfaces
{
    internal interface IConsoleBehavior
    {
        string Name { get; }
        Task ActAsync(string parameters);
    }
}
