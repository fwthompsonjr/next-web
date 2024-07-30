using Microsoft.AspNetCore.Http;
using next.web.core.models;

namespace next.web.core.extensions
{
    internal static class FormSubmissionExtensions
    {
        public static bool Validate(this FormSubmissionModel model, HttpRequest request)
        {
            if (model.IsValid) return true;
            var keys = request.Form.Keys.ToList();
            var isMapped = false;
            keys.ForEach(key =>
            {
                var tmp = key.ToInstance<FormSubmissionModel>();
                if (!isMapped && tmp != null)
                {
                    model.FormName = tmp.FormName;
                    model.Payload = tmp.Payload;
                    isMapped = true;
                }
            });
            return model.IsValid;
        }
    }
}
