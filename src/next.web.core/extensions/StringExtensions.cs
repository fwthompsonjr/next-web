using Newtonsoft.Json;

namespace next.web.core.extensions
{
    public static class StringExtensions
    {
        public static T? ToInstance<T>(this string str)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(str);
            }
            catch
            {
                return default;
            }
        }
    }
}
