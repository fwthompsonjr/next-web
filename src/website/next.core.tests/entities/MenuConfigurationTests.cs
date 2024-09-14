using Bogus;
using next.core.entities;

namespace next.core.tests.entities
{
    public class MenuConfigurationTests
    {
        private static readonly Faker<MenuConfigurationItem> itemfaker =
            new Faker<MenuConfigurationItem>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.IsDisplayed, y => y.Random.Bool())
            .RuleFor(x => x.IsSelected, y => y.Random.Bool());

        private readonly Faker<MenuConfiguration> faker =
            new Faker<MenuConfiguration>()
            .RuleFor(x => x.IsVisible, y => y.Random.Bool())
            .RuleFor(x => x.Items, y =>
            {
                var count = y.Random.Int(1, 6);
                return itemfaker.Generate(count);
            });

        [Fact]
        public void MenuConfigurationCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new MenuConfiguration();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void MenuConfigurationCanUpdateIsVisible()
        {
            var items = faker.Generate(2);
            items[0].IsVisible = items[1].IsVisible;
            Assert.Equal(items[1].IsVisible, items[0].IsVisible);
        }

        [Fact]
        public void MenuConfigurationCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Items = items[1].Items;
            Assert.Equal(items[1].Items, items[0].Items);
        }
    }
}