namespace next.web.core.reponses
{
    internal class FormSubmissionResponse
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? RedirectTo { get; set; }
        public string? OriginalFormName { get; set; }
    }
}
