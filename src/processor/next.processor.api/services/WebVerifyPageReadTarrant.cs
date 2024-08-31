namespace next.processor.api.services
{
    public class WebVerifyPageReadTarrant(IConfiguration configuration) : WebVerifyPageReadCollin(configuration)
    {
        protected override int WebId => 3;
    }
}