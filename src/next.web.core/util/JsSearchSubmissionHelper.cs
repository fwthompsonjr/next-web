using legallead.desktop.entities;

namespace next.web.core.util
{
    internal static class JsSearchSubmissionHelper
    {
        public static object Refine(object payload)
        {
            const StringComparison comparison = StringComparison.OrdinalIgnoreCase;
            if (payload is not BeginSearchModel model) return payload;
            if (!model.County.Name.Equals("Harris", comparison)) return model;
            model.Details.Clear();
            model.Details.Add(new() { Name = "Search Type", Text = "All Civil Courts", Value = "0" });
            return model;
        }
    }
}
