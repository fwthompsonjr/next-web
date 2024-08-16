using Microsoft.AspNetCore.Http;
using next.web.core.models;
using System.Diagnostics.CodeAnalysis;

namespace next.web.core.extensions
{
    internal static class FormSubmissionExtensions
    {
        public static bool Validate(this FormSubmissionModel model, HttpRequest request)
        {
            if (model.IsValid) return true;
            var form = request.Form;
            var keys = form?.Keys.ToList();
            if (form == null || keys == null || keys.Count == 0) return false;
            var isMapped = false;
            keys.ForEach(key =>
            {
                var tmp = key.ToInstance<FormSubmissionModel>();
                tmp ??= GetFormKeyValue(form, key).ToInstance<FormSubmissionModel>();
                if (!isMapped && tmp != null)
                {
                    model.FormName = tmp.FormName;
                    model.Payload = tmp.Payload;
                    isMapped = true;
                }
            });
            return model.IsValid;
        }
        [ExcludeFromCodeCoverage(Justification = "Null form edge cases are not needed for this private member.")]
        private static string GetFormKeyValue(IFormCollection form, string key)
        {
            var hasKey = form.TryGetValue(key, out var item);
            if (!hasKey || item.Count <= 0) return string.Empty;
            return item[0] ?? string.Empty;
        }
    }

}
