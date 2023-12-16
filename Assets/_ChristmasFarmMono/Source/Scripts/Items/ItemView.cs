using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Items
{
    [CreateAssetMenu(menuName = "ChristmasFarm / Items / Item View", order = 2)]
    public class ItemView : ScriptableObject, IItem
    {
        [SerializeField] private ItemIdentifier identifier;

        [field: SerializeField] public GameObject ItemPrefab { get; private set; }
        
        public string Identifier => identifier.Id;
    }
}