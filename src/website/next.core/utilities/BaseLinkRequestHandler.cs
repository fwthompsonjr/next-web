using next.core.entities;
using next.core.interfaces;
using System.Diagnostics.CodeAnalysis;

namespace next.core.utilities
{
    [ExcludeFromCodeCoverage(Justification = "Tested directly from subclass")]
    public abstract class BaseLinkRequestHandler : ILinkRequestHandler
    {
        public abstract T GetContent<T>(string url);

        public List<KeyNameBo> GetParameters(string url)
        {
            return DecodeQueryParameters(url);
        }
        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static List<KeyNameBo> DecodeQueryParameters(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return new();
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri)) return new();
            if (uri.Query.Length == 0) return new();
            var dictionary = uri.Query.TrimStart('?')
                            .Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
                            .GroupBy(parts => parts[0],
                                     parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length <= 1 ? "" : parts[1]))
                            .ToDictionary(grouping => grouping.Key,
                                          grouping => string.Join(",", grouping));
            return dictionary.Select(x => new KeyNameBo { Key = x.Key, Value = x.Value }).ToList();
        }
    }
}
