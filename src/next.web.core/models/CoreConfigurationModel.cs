using Microsoft.Extensions.Configuration;
using System.Text;

namespace next.web.core.models
{
    public class CoreConfigurationModel
    {
        public CoreConfigurationModel()
        {
            var json = Properties.Resources.core_configuration;
            GetConfiguration = new ConfigurationBuilder()
                .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json))).Build();
        }
        public IConfiguration GetConfiguration { get; private set; }
    }
}
