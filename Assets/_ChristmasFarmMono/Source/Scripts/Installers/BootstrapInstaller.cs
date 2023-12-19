using _ChristmasFarmMono.Source.Scripts.Inventory;
using _ChristmasFarmMono.Source.Scripts.ItemsDatabases;
using VContainer;
using VContainer.Unity;

namespace _ChristmasFarmMono.Source.Scripts.Installers
{
    public class BootstrapInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ItemsViewDatabase>(Lifetime.Singleton);
            builder.Register<ItemsViewUIDatabase>(Lifetime.Singleton);
            builder.Register<InventoryController>(Lifetime.Singleton);
        }
    }
}