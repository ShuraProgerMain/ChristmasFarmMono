using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Items
{
    [CreateAssetMenu(menuName = "ChristmasFarm / Items / Item View UI", order = 2)]
    public sealed class ItemViewUI : ScriptableObject, IItem
    {
        [SerializeField] private AnyIdentifier identifier;
        
        [field: SerializeField] public Sprite ItemSprite { get; private set; }
        
        public string Identifier => identifier.Id;
    }
}