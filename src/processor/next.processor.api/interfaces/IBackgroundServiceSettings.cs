namespace next.processor.api.interfaces
{
    public interface IBackgroundServiceSettings
    {
        bool Enabled { get; set; }
        int Delay { get; set; }
        int Interval { get; set; }
    }
}
