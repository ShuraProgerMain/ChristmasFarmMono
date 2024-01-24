using _ChristmasFarmMono.Source.Scripts.Configs;
using _ChristmasFarmMono.Source.Scripts.Forest.FirTree;
using _ChristmasFarmMono.Source.Scripts.GardenBed;
using _ChristmasFarmMono.Source.Scripts.InHandObjects.InHandsObjectsInventory;
using _ChristmasFarmMono.Source.Scripts.Inventory;
using _ChristmasFarmMono.Source.Scripts.ItemsDatabases;
using _ChristmasFarmMono.Source.Scripts.Player;
using _ChristmasFarmMono.Source.Scripts.UI;
using VContainer;
using VContainer.Unity;

namespace _ChristmasFarmMono.Source.Scripts.Installers
{
    public class BootstrapInstaller : LifetimeScope
    {
        private GameConfigs _gameConfigs;
        protected override async void Awake()
        {
            var creator = new GameConfigCreator();
            _gameConfigs = await creator.LoadConfigs();
            
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_gameConfigs);
            builder.Register<InputActionsService>(Lifetime.Singleton);
            builder.Register<ItemsViewDatabase>(Lifetime.Singleton);
            builder.Register<ItemsViewUIDatabase>(Lifetime.Singleton);
            builder.Register<InventoryController>(Lifetime.Singleton);

            builder.RegisterComponentInHierarchy<GardenBedsController>();
            builder.RegisterComponentInHierarchy<ItemsHolderShow>();
            builder.RegisterComponentInHierarchy<FirTreesController>();
            builder.RegisterComponentInHierarchy<InHandsObjectInventoryController>();
            builder.RegisterComponentInHierarchy<PlayerController>().AsImplementedInterfaces();
        }
    }
}