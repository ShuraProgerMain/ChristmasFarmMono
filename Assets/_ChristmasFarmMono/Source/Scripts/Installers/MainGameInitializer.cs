using _ChristmasFarmMono.Source.Scripts.Configs;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Installers
{
    public class MainGameInitializer : MonoBehaviour
    {
        [SerializeField] private BootstrapInstaller bootstrapInstaller;

        public void Awake()
        {
            bootstrapInstaller.Build();
        }
    }
}
