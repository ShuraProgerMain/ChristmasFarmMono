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
    
    public class GardenBedMediator : MonoBehaviour, IInteractive, ISelectable
    {
        private Material _materialVariant;
        
        private readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            _materialVariant = GetComponent<MeshRenderer>().material;
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
            
        }
    }
}