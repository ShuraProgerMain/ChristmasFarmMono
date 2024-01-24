using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects
{
    [CreateAssetMenu(menuName = "ChristmasFarm / In Hand Configs / Handled Object View", order = 2)]
    public sealed class HandledObjectViewConfig : ScriptableObject
    {
        [field: SerializeField] public GameObject HandledObjectView { get; private set; }
    }
}