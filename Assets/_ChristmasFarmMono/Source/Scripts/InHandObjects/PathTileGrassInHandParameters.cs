using System;
using System.Collections.Generic;
using _ChristmasFarmMono.Source.Scripts.Items;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects
{
    public enum PathTileType
    {
        Straight = 1,
        Angle = 2,
        Triple = 3,
        Cross = 4,
        End = 5
    }
    
    [Serializable]
    public class PathTile
    {
        [field: SerializeField] public PathTileType PathTileType { get; private set; }
        [field: SerializeField] public GameObject PathVariant { get; private set; }
    }
    
    [Serializable]
    public class PathTileGroup
    {
        [field: SerializeField] public AnyIdentifier GroupIdentifier { get; private set; }
        [field: SerializeField] public PathTile[] PathVariants { get; private set; }
    }
    
    [Serializable]
    public sealed record PathTilesItemConfig
    {
        [field: SerializeField] public PathTileGroup[] PathTileGroups { get; private set; }
    }

    public sealed class PathTileGroupsConfig
    {
        public Dictionary<string, Dictionary<PathTileType, PathTile>> PathTiles;
    }
    
    [CreateAssetMenu(menuName = "ChristmasFarm / In Hand Configs / PathTileGrass", order = 3)]
    public sealed class PathTileGrassInHandParameters : ScriptableObject
    {
        [field: SerializeField] public PathTilesItemConfig PathTilesItemConfig { get; private set; }
    }
}