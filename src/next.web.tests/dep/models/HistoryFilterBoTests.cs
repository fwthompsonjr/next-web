using Bogus;
using next.web.core.models;

namespace next.web.tests.dep.models
{
    public class HistoryFilterBoTests
    {
        private readonly static Faker<HistoryFilterBo> faker =
            new Faker<HistoryFilterBo>()
            .RuleFor(x => x.StatusId, y => y.Random.Int(1, 5000))
            .RuleFor(x => x.CountyId, y => y.Random.Int(1, 5000));

        [Fact]
        public void ModelCanBeGenerated()
        {
            var error = Record.Exception(() =>
            {
                var sut = faker.Generate(2);
                Assert.NotEqual(sut[0].StatusId, sut[1].StatusId);
                Assert.NotEqual(sut[0].CountyId, sut[1].CountyId);
            });
            Assert.Null(error);
        }
    }
}