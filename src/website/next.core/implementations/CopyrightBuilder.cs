using next.core.interfaces;

namespace next.core.implementations
{
    internal class CopyrightBuilder : ICopyrightBuilder
    {
        private const int initialYear = 2023;
        private const string defaultProductName = "Legal Lead UI";
        private readonly string productName;
        private readonly List<int> listYears = new() { initialYear };

        public CopyrightBuilder(
            DateTime? current = null,
            string? product = defaultProductName)
        {
            var dtnow = current.GetValueOrDefault(DateTime.UtcNow);
            int currentYear = dtnow.Year;
            productName = product ?? defaultProductName;
            AppendIfMissing(currentYear);
            BuildAndSortDates();
        }

        public string GetCopyright()
        {
            var collection = GetYears();
            var years = collection.Select(a => Convert.ToString(a));
            var yrlist = string.Join(", ", years);
            var copy = $"(c) {yrlist} {productName}";
            return copy;
        }

        public List<int> GetYears()
        {
            AppendIfMissing(DateTime.UtcNow.Year);
            BuildAndSortDates();
            return new List<int>(listYears);
        }

        private void BuildAndSortDates()
        {
            int mx = listYears.Max();
            int mn = listYears.Min();
            while (mx > mn)
            {
                AppendIfMissing(mx);
                mx--;
            }
            listYears.Sort();
        }

        private void AppendIfMissing(int year)
        {
            if (!listYears.Contains(year))
            {
                listYears.Add(year);
            }
        }
    }
}