namespace next.processor.api.services
{
    public static class DataService
    {
        public static void ReportState(string status)
        {
            if (string.IsNullOrWhiteSpace(status)) return;
            Console.WriteLine(status);
        }
        public static void ReportHealth(string health)
        {
            if (string.IsNullOrWhiteSpace(health)) return;
            Console.WriteLine(health);
        }
    }
}
