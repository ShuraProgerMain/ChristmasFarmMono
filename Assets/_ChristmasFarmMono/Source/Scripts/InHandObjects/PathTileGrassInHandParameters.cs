using System;
using _ChristmasFarmMono.Source.Scripts.Items;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects
{
    [Serializable]
    public class PathTileGroup
    {
        [field: SerializeField] public AnyIdentifier GroupIdentifier { get; private set; }
        [field: SerializeField] public GameObject[] PathVariants { get; private set; }
    }
    
    [Serializable]
    public sealed record PathTilesItemConfig
    {
        [field: SerializeField] public PathTileGroup[] PathTileGroups { get; private set; }
    }
    
    [CreateAssetMenu(menuName = "ChristmasFarm / In Hand Configs / PathTileGrass", order = 3)]
    public sealed class PathTileGrassInHandParameters : ScriptableObject
    {
        [field: SerializeField] public PathTilesItemConfig PathTilesItemConfig { get; private set; }
    }
}