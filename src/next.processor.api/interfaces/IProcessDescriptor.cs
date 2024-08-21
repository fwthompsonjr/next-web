namespace next.processor.api.interfaces
{
    public interface IProcessDescriptor
    {
        int? Id { get; set; }
        string Name { get; set; }
        string Descriptor { get; set; }
    }
}
