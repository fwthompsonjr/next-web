using HtmlAgilityPack;
using Newtonsoft.Json;

namespace next.web.core.extensions
{
    public static class StringExtensions
    {
        public static string ToJsonString(this object? obj)
        {
            if (obj == null) return string.Empty;
            return JsonConvert.SerializeObject(obj);
        }
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
        public static object? ToInstance(this string str, Type type)
        {
            try
            {
                return JsonConvert.DeserializeObject(str, type);
            }
            catch
            {
                return default;
            }
        }

        public static HtmlDocument ToHtml(this string str)
        {
            var document = new HtmlDocument();
            document.LoadHtml(str);
            return document;
        }
    }
}
