using System;
using _ChristmasFarmMono.Source.Scripts.InHandObjects.PathTilesInHand;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects.InHandsObjectsInventory
{
    [CreateAssetMenu(fileName = "PathTileReference", menuName = "ChristmasFarm / In Hand References / PathTileReference")]
    public sealed class InHandsObjectReferencePathTile : InHandsObjectReference
    {
        public override Type HandheldObjectType => typeof(PathTileInHand);
    }
}