using next.core.implementations;

namespace next.core.tests.implementations
{
    public class CopyrightBuilderTests
    {
        private const int initialYear = 2023;
        private const string myProduct = "Legal Lead UI";
        private readonly CopyrightBuilder builder = new();

        [Fact]
        public void BuilderIsCreated()
        {
            Assert.NotNull(builder);
        }

        [Fact]
        public void BuilderContainsInitialYear()
        {
            var list = builder.GetYears();
            Assert.Contains(initialYear, list);
        }

        [Fact]
        public void BuilderContainsCurrentYear()
        {
            var currentYear = DateTime.UtcNow.Year;
            var list = builder.GetYears();
            Assert.Contains(currentYear, list);
        }

        [Fact]
        public void BuilderCopyrightContainsCurrentYear()
        {
            var currentYear = DateTime.UtcNow.Year.ToString();
            var list = builder.GetCopyright();
            Assert.Contains(currentYear, list);
        }

        [Fact]
        public void BuilderCopyrightContainsInitialYear()
        {
            var currentYear = initialYear.ToString();
            var list = builder.GetCopyright();
            Assert.Contains(currentYear, list);
        }

        [Fact]
        public void BuilderCopyrightContainsMyProduct()
        {
            var list = builder.GetCopyright();
            Assert.Contains(myProduct, list);
        }

        [Fact]
        public void BuilderCopyrightContainsDefaultProduct()
        {
            var instance = new CopyrightBuilder(DateTime.UtcNow, null);
            var list = instance.GetCopyright();
            Assert.Contains(myProduct, list);
        }

        [Theory]
        [InlineData(2025)]
        [InlineData(2026)]
        [InlineData(2027)]
        public void BuilderCopyrightSupportsFutureDates(int year)
        {
            var startDate = new DateTime(year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var count = year - initialYear + 1;
            var instance = new CopyrightBuilder(startDate);
            var list = instance.GetYears();
            Assert.Contains(year, list);
            Assert.Equal(count, list.Count);
        }

        [Theory]
        [InlineData("Product 1")]
        [InlineData("Product 2")]
        [InlineData("Product 3")]
        public void BuilderCopyrightSupportsAlternateProductName(string name)
        {
            var startDate = DateTime.UtcNow;
            var instance = new CopyrightBuilder(startDate, name);
            var list = instance.GetCopyright();
            Assert.Contains(name, list);
        }
    }
}