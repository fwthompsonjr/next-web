using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class BeginSearchModelTests
    {
        private static readonly Faker<BeginSearchDetail> dfaker
            = new Faker<BeginSearchDetail>()
            .RuleFor(x => x.Value, y => y.Random.String(5, 500))
            .RuleFor(x => x.Text, y => y.Random.String(5, 500))
            .RuleFor(x => x.Name, y => y.Random.String(5, 500));

        private static readonly Faker<BeginSearchModel> faker
            = new Faker<BeginSearchModel>()
            .RuleFor(x => x.State, y => y.Random.String(5, 10))
            .RuleFor(x => x.Start, y => y.Random.Int(200, 300))
            .RuleFor(x => x.End, y => y.Random.Int(300, 400))
            .FinishWith((a, b) =>
            {
                var nbr = a.Random.Int(2, 6);
                b.Details = dfaker.Generate(nbr);
            });

        [Fact]
        public void ModelCanBeGenerated()
        {
            var original = new BeginSearchModel();
            var test = faker.Generate();
            Assert.NotEqual(original.State, test.State);
            Assert.NotEqual(original.County, test.County);
            Assert.NotEqual(original.Start, test.Start);
            Assert.NotEqual(original.End, test.End);
        }
    }
}