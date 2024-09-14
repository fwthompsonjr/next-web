using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class CboDropDownModelTests
    {
        private static readonly Faker<DropDownModel> ddFaker =
            new Faker<DropDownModel>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Name, y => y.Person.FullName);

        private static readonly Faker<CboDropDownModel> faker =
            new Faker<CboDropDownModel>()
            .RuleFor(x => x.Id, y => y.Random.Int(10, 10000))
            .RuleFor(x => x.Members, y =>
            {
                var count = y.Random.Int(1, 10);
                return ddFaker.Generate(count);
            })
            .RuleFor(x => x.Name, y => y.Person.FullName);

        [Fact]
        public void ModelCanConstruct()
        {
            var exception = Record.Exception(() =>
            {
                _ = new CboDropDownModel();
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
        public void ModelCanGetId()
        {
            var test = faker.Generate();
            Assert.NotEqual(0, test.Id);
        }

        [Fact]
        public void ModelCanSetId()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Id = source.Id;
            Assert.Equal(source.Id, test.Id);
        }

        [Fact]
        public void ModelCanGetName()
        {
            var test = faker.Generate();
            Assert.False(string.IsNullOrEmpty(test.Name));
        }

        [Fact]
        public void ModelCanSetName()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Name = source.Name;
            Assert.Equal(source.Name, test.Name);
        }

        [Fact]
        public void ModelCanGetMembers()
        {
            var test = faker.Generate();
            Assert.NotEmpty(test.Members);
        }

        [Fact]
        public void ModelCanSetMembers()
        {
            var test = faker.Generate();
            var source = faker.Generate();
            test.Members = source.Members;
            Assert.Equal(source.Members, test.Members);
        }
    }
}