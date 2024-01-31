using System.Threading.Tasks;
using _ChristmasFarmMono.Source.Scripts.InHandObjects;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _ChristmasFarmMono.Source.Scripts.Configs
{
    // Переписать это в эдитор скрипт, наверное. А в рантайме использовать уже просто JSON
    public interface IConfigLoader<T>
    {
        public Task<T> LoadConfig();
    }

    public sealed class LoadGardenBedInHandConfig : IConfigLoader<GardenBedItemConfig>
    {
        public async Task<GardenBedItemConfig> LoadConfig()
        {
            GardenBedInHandParameters config = await Addressables.LoadAssetAsync<GardenBedInHandParameters>(AddressableExtensions.GameConfigs.GardenBedInHandConfig).Task;

            return config.ItemConfig with { };
        }
    }
    
    public sealed class LoadPathTileGrassInHandConfig : IConfigLoader<PathTilesItemConfig>
    {
        public async Task<PathTilesItemConfig> LoadConfig()
        {
            PathTileGrassInHandParameters config = await Addressables.LoadAssetAsync<PathTileGrassInHandParameters>(AddressableExtensions.GameConfigs.PathTiles).Task;

            return config.PathTilesItemConfig with { };
        }
    }
    
    public sealed class GameConfigCreator
    {
        public async Task<GameConfigs> LoadConfigs()
        {
            var gameConfigs = new GameConfigs
            {
                GardenBedItemConfig = await new LoadGardenBedInHandConfig().LoadConfig(),
                PathTilesItemConfig = await new LoadPathTileGrassInHandConfig().LoadConfig(),
            };

            return gameConfigs;
        }
    }

    public sealed class GameConfigs
    {
        [CanBeNull] public GardenBedItemConfig GardenBedItemConfig;
        [CanBeNull] public PathTilesItemConfig PathTilesItemConfig;
    }
}