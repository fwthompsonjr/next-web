namespace next.processor.api.models
{
    public class NamedServiceSetting
    {
        public string Name { get; set; } = string.Empty;
        public ServiceSettings Setting { get; set; } = new();
    }
}
