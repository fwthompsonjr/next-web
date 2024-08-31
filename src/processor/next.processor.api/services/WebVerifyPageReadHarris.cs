namespace next.processor.api.services
{
    public class WebVerifyPageReadHarris(IConfiguration configuration) : WebVerifyPageReadCollin(configuration)
    {
        protected override int WebId => 2;
    }
}