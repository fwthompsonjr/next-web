using Microsoft.Extensions.Configuration;

namespace next.processor.api.services
{
    public class WebVerifyPageReadHarrisJp(IConfiguration configuration) : WebVerifyPageReadCollin(configuration)
    {
        protected override int WebId => 4;
    }
}