using next.core.entities;

namespace next.core.interfaces
{
    public interface ILinkRequestHandler
    {
        List<KeyNameBo> GetParameters(string url);
        T GetContent<T>(string url);
    }
}
