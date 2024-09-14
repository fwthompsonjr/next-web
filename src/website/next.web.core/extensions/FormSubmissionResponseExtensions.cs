using next.core.entities;
using next.web.core.reponses;

namespace next.web.core.extensions
{
    internal static class FormSubmissionResponseExtensions
    {
        public static void MapResponse(this FormSubmissionResponse obj, ApiResponse source)
        {
            obj.StatusCode = source.StatusCode;
            obj.Message = source.Message;
        }
    }
}
