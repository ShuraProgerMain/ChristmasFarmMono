using System;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects.InHandsObjectsInventory
{
    [CreateAssetMenu(fileName = "GardenBedReference", menuName = "ChristmasFarm / In Hand References / Garden bed reference")]
    public sealed class InHandsObjectReferenceGardenBed : InHandsObjectReference
    {
        public override Type HandheldObjectType => typeof(GardenBedInHand);
    }
}