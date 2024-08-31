namespace next.processor.api.services
{
    public class WebVerifyPageReadDenton(IConfiguration configuration) : WebVerifyPageReadCollin(configuration)
    {
        protected override int WebId => 1;
    }
}
