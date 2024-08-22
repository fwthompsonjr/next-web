using Bogus;
using next.processor.api.models;

namespace next.processor.api.tests.models
{
    public class QueuePersonItemTests
    {
        [Fact]
        public void QueuePersonItemCanBeCreated()
        {
            var error = Record.Exception(() =>
            {
                _ = new QueuePersonItem();
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueuePersonItemCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                _ = faker.Generate(10);
            });
            Assert.Null(error);
        }

        [Fact]
        public void QueuePersonItemFieldsCanGetAndSet()
        {
            var error = Record.Exception(() =>
            {
                var collection = faker.Generate(2);
                var a = collection[0];
                var b = collection[1];
                Assert.NotEqual(a.Name, b.Name);
                Assert.NotEqual(a.Zip, b.Zip);
                Assert.NotEqual(a.Address1, b.Address1);
                Assert.NotEqual(a.Address2, b.Address2);
                Assert.NotEqual(a.Address3, b.Address3);
                Assert.NotEqual(a.CaseNumber, b.CaseNumber);
                Assert.NotEqual(a.DateFiled, b.DateFiled);
                Assert.NotEqual(a.Court, b.Court);
                Assert.NotEqual(a.CaseType, b.CaseType);
                Assert.NotEqual(a.CaseStyle, b.CaseStyle);
                Assert.NotEqual(a.FirstName, b.FirstName);
                Assert.NotEqual(a.LastName, b.LastName);
                Assert.NotEqual(a.Plantiff, b.Plantiff);
                Assert.NotEqual(a.Status, b.Status);
                _ = a.IsValid;
            });
            Assert.Null(error);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void QueuePersonItemCanValidate(int fieldId)
        {
            var error = Record.Exception(() =>
            {
                var test = faker.Generate();
                if (fieldId == 0) test.Name = string.Empty;
                if (fieldId == 1) test.Zip = string.Empty;
                if (fieldId == 2) test.Address1 = string.Empty;
                var expected = fieldId > 2;
                var actual = test.IsValid;
                Assert.Equal(expected, actual);
            });
            Assert.Null(error);
        }

        private static readonly Faker<QueuePersonItem> faker =
            new Faker<QueuePersonItem>()
            .RuleFor(x => x.Name, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Zip, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Address1, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Address2, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Address3, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseNumber, y => y.Random.Guid().ToString())
            .RuleFor(x => x.DateFiled, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Court, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseType, y => y.Random.Guid().ToString())
            .RuleFor(x => x.CaseStyle, y => y.Random.Guid().ToString())
            .RuleFor(x => x.FirstName, y => y.Random.Guid().ToString())
            .RuleFor(x => x.LastName, y => y.Random.Guid().ToString())
            .RuleFor(x => x.Plantiff, y => y.Random.AlphaNumeric(250))
            .RuleFor(x => x.Status, y => y.Random.Int(1, 20000).ToString());
    }
}
