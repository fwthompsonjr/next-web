using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class CountySearchConfigurationTests
    {
        private static readonly Faker<CaseSearchModel> searchFaker =
            new Faker<CaseSearchModel>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Name, y => y.Person.FullName);

        private static readonly Faker<DropDownModel> ddFaker =
            new Faker<DropDownModel>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Name, y => y.Person.FullName);

        private static readonly Faker<CboDropDownModel> cbofaker =
            new Faker<CboDropDownModel>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Members, y =>
            {
                var count = y.Random.Int(1, 10);
                return ddFaker.Generate(count);
            })
            .RuleFor(x => x.Name, y => y.Person.FullName);

        private static readonly Faker<CountySearchDetail> detailfaker =
            new Faker<CountySearchDetail>()
            .RuleFor(x => x.DropDowns, y =>
            {
                var count = y.Random.Int(1, 10);
                return cbofaker.Generate(count).ToArray();
            })
            .RuleFor(x => x.CaseSearchTypes, y =>
            {
                var count = y.Random.Int(1, 10);
                return searchFaker.Generate(count).ToArray();
            });

        private static readonly Faker<CountySearchConfiguration> faker =
            new Faker<CountySearchConfiguration>()
            .RuleFor(x => x.Index, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Name, y => y.Person.FullName)
            .RuleFor(x => x.StateCode, y => y.Random.AlphaNumeric(2))
            .RuleFor(x => x.ShortName, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.IsActive, y => y.Random.Bool())
            .RuleFor(x => x.Data, y => detailfaker.Generate());

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new CountySearchConfiguration();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanGenerate()
        {
            var exception = Record.Exception(() =>
            {
                _ = faker.Generate();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void ModelCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void ModelCanUpdateId()
        {
            var items = faker.Generate(2);
            items[0].Index = items[1].Index;
            Assert.Equal(items[1].Index, items[0].Index);
        }

        [Fact]
        public void ModelCanUpdateShortName()
        {
            var items = faker.Generate(2);
            items[0].ShortName = items[1].ShortName;
            Assert.Equal(items[1].ShortName, items[0].ShortName);
        }

        [Fact]
        public void ModelCanUpdateStateCode()
        {
            var items = faker.Generate(2);
            items[0].StateCode = items[1].StateCode;
            Assert.Equal(items[1].StateCode, items[0].StateCode);
        }

        [Fact]
        public void ModelCanUpdateIsActive()
        {
            var items = faker.Generate(2);
            items[0].IsActive = items[1].IsActive;
            Assert.Equal(items[1].IsActive, items[0].IsActive);
        }

        [Fact]
        public void ModelCanGetCaseSearchModels()
        {
            var test = faker.Generate();
            Assert.NotNull(test.Data);
        }

        [Fact]
        public void ModelCanSetData()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Data = source.Data;
            Assert.Equal(source.Data, test.Data);
        }
    }
}