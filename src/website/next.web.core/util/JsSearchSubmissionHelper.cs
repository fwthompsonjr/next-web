using next.core.entities;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.util
{
    [ExcludeFromCodeCoverage(Justification = "Integration only. Might cover at later date.")]
    internal static class JsSearchSubmissionHelper
    {
        public static object Refine(object payload)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (payload is not BeginSearchModel model) return payload;
            var countyName = model.County.Name;
            if (!countyName.Equals("Harris", comparison)) return model;
            model.Details.Clear();
            model.Details.Add(new() { Name = "Search Type", Text = "All Civil Courts", Value = "0" });
            return model;
        }
    }
}
