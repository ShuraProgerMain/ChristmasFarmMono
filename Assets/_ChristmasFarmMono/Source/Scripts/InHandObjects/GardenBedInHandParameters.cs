using System;
using _ChristmasFarmMono.Source.Scripts.GardenBed;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects
{
    [Serializable]
    public sealed record GardenBedItemConfig
    {
        [field: SerializeField] public GardenBedMediator GardenBedMediator { get; private set; }
        [field: SerializeField] public Vector2 CellOffset { get; private set; }
    }
    [CreateAssetMenu(menuName = "ChristmasFarm / In Hand Configs / GardenBed", order = 2)]
    public sealed class GardenBedInHandParameters : ScriptableObject
    {
        public GardenBedItemConfig ItemConfig;
    }
}