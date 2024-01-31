using System;
using _ChristmasFarmMono.Source.Scripts.Items;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects.InHandsObjectsInventory
{
    public abstract class InHandsObjectReference : ScriptableObject
    {
        [field: SerializeField] public AnyIdentifier Identifier { get; private set; }

        public abstract Type HandheldObjectType { get; }
    }
}