using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Items
{
    [CreateAssetMenu(menuName = "ChristmasFarm / Items / Item View UI", order = 2)]
    public class ItemViewUI : ScriptableObject, IItem
    {
        [SerializeField] private ItemIdentifier identifier;
        
        [field: SerializeField] public Sprite ItemSprite { get; private set; }
        
        public string Identifier => identifier.Id;
    }
}