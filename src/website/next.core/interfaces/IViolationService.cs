using next.core.entities;

namespace next.core.interfaces
{
    public interface IViolationService
    {
        void Append(ViolationBo incident);
        void Expire();
        bool IsViolation(ViolationBo incident);
    }
}
