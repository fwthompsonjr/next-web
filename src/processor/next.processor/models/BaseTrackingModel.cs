using next.processor.api.interfaces;

namespace next.processor.api.models
{
    public abstract class BaseTrackingModel : ITrackable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime ExpirationDate { get; set; }
    }
}
