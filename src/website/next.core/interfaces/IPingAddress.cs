using System.Net.NetworkInformation;

namespace next.core.interfaces
{
    public interface IPingAddress
    {
        IPStatus CheckStatus(string address);
    }
}