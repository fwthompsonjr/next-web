using next.processor.api.interfaces;

namespace next.processor.api.models
{
    public class ServiceSettings : IBackgroundServiceSettings
    {
        public bool Enabled { get; set; }
        public int Delay { get; set; }
        public int Interval { get; set; }
    }
}
