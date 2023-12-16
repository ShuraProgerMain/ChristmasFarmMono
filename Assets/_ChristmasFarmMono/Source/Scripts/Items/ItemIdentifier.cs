using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Items
{
    [CreateAssetMenu(menuName = "ChristmasFarm / Items / Item Identifier", order = 0)]
    public class ItemIdentifier : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
    }
}