using next.web.core.reponses;

namespace next.web.core.util
{
    internal static class FormResponses
    {
        public static readonly FormSubmissionResponse Default = new()
        {
            StatusCode = 422,
            OriginalFormName = "",
            Message = "Unable to process request. Please check values and retry.",
            RedirectTo = "/home"
        };

        public static FormSubmissionResponse GetDefault(string? formName)
        {
            var response = Copy(Default);
            response.OriginalFormName = formName ?? string.Empty;
            return response;
        }


        private static FormSubmissionResponse Copy(FormSubmissionResponse response)
        {
            return new FormSubmissionResponse
            {
                StatusCode = response.StatusCode,
                OriginalFormName = response.OriginalFormName,
                Message = response.Message,
                RedirectTo = response.RedirectTo,
            };
        }
    }
}
