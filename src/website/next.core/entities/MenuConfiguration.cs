namespace next.core.entities
{
    internal class MenuConfiguration
    {
        public bool IsVisible { get; set; }
        public List<MenuConfigurationItem> Items { get; set; } = new();
    }
}