using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class DiscountChoiceTests
    {
        private readonly Faker<DiscountChoice> faker =
            new Faker<DiscountChoice>()
            .RuleFor(x => x.IsSelected, y => y.Random.Bool())
            .RuleFor(x => x.StateName, y => y.Company.CompanyName())
            .RuleFor(x => x.CountyName, y => y.Company.CompanyName());

        [Fact]
        public void DiscountChoiceCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new DiscountChoice();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void DiscountChoiceCanUpdateIsSelected()
        {
            var items = faker.Generate(2);
            items[0].IsSelected = items[1].IsSelected;
            Assert.Equal(items[1].IsSelected, items[0].IsSelected);
        }

        [Fact]
        public void DiscountChoiceCanUpdateCountyName()
        {
            var items = faker.Generate(2);
            items[0].CountyName = items[1].CountyName;
            Assert.Equal(items[1].CountyName, items[0].CountyName);
        }

        [Fact]
        public void DiscountChoiceCanUpdateStateName()
        {
            var items = faker.Generate(2);
            items[0].StateName = items[1].StateName;
            Assert.Equal(items[1].StateName, items[0].StateName);
        }
    }
}