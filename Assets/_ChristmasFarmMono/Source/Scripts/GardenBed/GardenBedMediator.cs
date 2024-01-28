using Unity.Mathematics;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed class GardenBedMediator : MonoBehaviour, IInteractive, ISelectable
    {
        private string _identifier;
        private Material _materialVariant;

        private GardenBedsBehaviourManager _gardenBedsBehaviourManager;
        private GameObject _currentPlantItem;
        
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        public string Identifier => _identifier;

        public void Initialize(GardenBedsBehaviourManager gardenBedsBehaviourManager, string identifier)
        {
            enabled = true;
            _identifier = identifier;
            _materialVariant = GetComponent<MeshRenderer>().material;

            GetComponent<Collider>().enabled = true;

            _gardenBedsBehaviourManager = gardenBedsBehaviourManager;
        }

        public void SetItem(GameObject item)
        {
            _currentPlantItem = Instantiate(item, transform.position, 
                quaternion.identity);
            _currentPlantItem.transform.SetParent(transform);
        }
        
        public void ClearGardenBed()
        {
            Destroy(_currentPlantItem);
        }

        public void Select()
        {
            _materialVariant.SetFloat(OutlineWidth, value: .5f);
            _materialVariant.SetColor(BaseColor, Color.blue);
        }

        public void DropSelect()
        {
            _materialVariant.SetFloat(OutlineWidth, value: 0);
            _materialVariant.SetColor(BaseColor, Color.white);
        }

        public void Interact()
        {
            _gardenBedsBehaviourManager.Interactive(Identifier);
        }
    }
}