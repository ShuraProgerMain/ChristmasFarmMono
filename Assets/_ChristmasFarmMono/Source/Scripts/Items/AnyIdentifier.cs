using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Items
{
    [CreateAssetMenu(menuName = "ChristmasFarm / Any / Item Identifier", order = 0)]
    public sealed class AnyIdentifier : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }

        public override string ToString()
        {
            return $"Identifier {Id}";
        }
    }
}