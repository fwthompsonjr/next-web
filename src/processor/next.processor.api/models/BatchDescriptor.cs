using next.processor.api.interfaces;

namespace next.processor.api.models
{
    public class BatchDescriptor : IProcessDescriptor
    {
        public int? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Descriptor { get; set; } = string.Empty;
    }
}
