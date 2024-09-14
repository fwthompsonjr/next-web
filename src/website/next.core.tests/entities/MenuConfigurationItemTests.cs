using Bogus;
using next.core.entities;
using next.core.utilities;
using Microsoft.Extensions.DependencyInjection;

namespace next.core.tests.entities
{
    public class MenuConfigurationItemTests
    {
        private static readonly Faker<MenuConfigurationItem> faker =
            new Faker<MenuConfigurationItem>()
            .RuleFor(x => x.Name, y => y.Random.AlphaNumeric(10))
            .RuleFor(x => x.IsDisplayed, y => y.Random.Bool())
            .RuleFor(x => x.IsSelected, y => y.Random.Bool());

        [Fact]
        public void MenuConfigurationItemCanBeCreated()
        {
            var exception = Record.Exception(() =>
            {
                _ = new MenuConfigurationItem();
            });
            Assert.Null(exception);
        }

        [Fact]
        public void MenuConfigurationItemCanUpdateIsDisplayed()
        {
            var items = faker.Generate(2);
            items[0].IsDisplayed = items[1].IsDisplayed;
            Assert.Equal(items[1].IsDisplayed, items[0].IsDisplayed);
        }

        [Fact]
        public void MenuConfigurationItemCanUpdateIsSelected()
        {
            var items = faker.Generate(2);
            items[0].IsSelected = items[1].IsSelected;
            Assert.Equal(items[1].IsSelected, items[0].IsSelected);
        }

        [Fact]
        public void MenuConfigurationItemCanUpdateName()
        {
            var items = faker.Generate(2);
            items[0].Name = items[1].Name;
            Assert.Equal(items[1].Name, items[0].Name);
        }

        [Fact]
        public void ProvderCanGetMenu()
        {
            var menu = DesktopCoreServiceProvider.Provider?.GetRequiredService<MenuConfiguration>();
            Assert.NotNull(menu);
        }

        [Fact]
        public void ProvderCanGetItemCollection()
        {
            var menu = DesktopCoreServiceProvider.Provider?.GetRequiredService<MenuConfiguration>();
            Assert.NotNull(menu);
            Assert.NotEmpty(menu.Items);
        }
    }
}