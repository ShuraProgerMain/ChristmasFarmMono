using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects.InHandsObjectsInventory
{
    [CreateAssetMenu(fileName = "Hands object reference", menuName = "ChristmasFarm / In Hand References / Garden bed reference")]
    public sealed class InHandsObjectReferenceGardenBed : InHandsObjectReference
    {
        [FormerlySerializedAs("_gardenBedInHandConfig")] [SerializeField] private GardenBedItemConfig gardenBedItemConfig;
        public override Type HandheldObjectType => typeof(GardenBedInHand);
    }
}