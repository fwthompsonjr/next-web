using System.Diagnostics.CodeAnalysis;

namespace next.core.utilities
{
    internal static class DownloadStatusMessaging
    {
        public static string GetMessage(int statusCode, string description)
        {
            var enline = Environment.NewLine;
            var heading = $"Status code: {statusCode}<br/>";
            var subheading = statusCode switch
            {
                206 => "File Process Incomplete<br/>",
                400 => "Bad Request<br/>",
                401 => "Unauthorized<br/>",
                402 => "Payment Required<br/>",
                422 => "Unprocessable Content<br/>",
                _ => "Unexpected Error<br/>"
            };
            description = CollapseServerResponse(description);
            var message = string.Concat(
                heading,
                enline,
                subheading,
                enline,
                description);
            return message;
        }

        [ExcludeFromCodeCoverage(Justification = "Private member tested from public method.")]
        private static string CollapseServerResponse(string original)
        {
            if (string.IsNullOrWhiteSpace(original)) return "An error occurred processing your request";
            if (original.Contains("<html>")) return "Invalid request, please check submission or retry";
            if (original.Length > 250) return original[250..];
            return original;
        }
    }

}
