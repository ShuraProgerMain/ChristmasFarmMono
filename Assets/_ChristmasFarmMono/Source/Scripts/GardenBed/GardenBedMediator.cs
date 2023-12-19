using _ChristmasFarmMono.Source.Scripts.Items;
using Unity.Mathematics;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public interface IInteractive
    {
        public void Interact();
    }
    
    public interface ISelectable
    {
        public void Select();
        public void DropSelect();
    }
    
    public sealed class GardenBedMediator : MonoBehaviour, IInteractive, ISelectable
    {
        [SerializeField] private AnyIdentifier identifier;
        private Material _materialVariant;

        private GardenBedsController _gardenBedsController;
        private GameObject _currentPlantItem;
        
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        public string Identifier => identifier.Id;

        public void Initialize(GardenBedsController gardenBedsController)
        {
            _materialVariant = GetComponent<MeshRenderer>().material;

            _gardenBedsController = gardenBedsController;
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
            Debug.Log($"GardenBed {Identifier} interact");
            _gardenBedsController.Interactive(Identifier);
        }
    }
}