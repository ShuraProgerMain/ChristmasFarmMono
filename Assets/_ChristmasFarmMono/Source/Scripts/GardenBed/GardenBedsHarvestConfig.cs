using _ChristmasFarmMono.Source.Scripts.Items;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    [System.Serializable]
    public class ValuePair<TFirst, TSecond>
    {
        [field: SerializeField] public TFirst First { get; private set; }
        [field: SerializeField] public TSecond Second { get; private set; }
    }
    
    [CreateAssetMenu(fileName = "HarvestConfig", menuName = "ChristmasFarm / Garden beds / Garden bed harvest config", order = 0)]
    public class GardenBedsHarvestConfig : ScriptableObject
    {
        [field: SerializeField] public ValuePair<AnyIdentifier, int>[] HarvestAmounts { get; private set; }
    }
}