using _ChristmasFarmMono.Source.Scripts.GardenBed;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Forest.FirTree
{
    public class FirTreeMediator : MonoBehaviour, IInteractive, ISelectable
    {
        private string _identifier;
        private Material _materialVariant;
        
        private static readonly int OutlineWidth = Shader.PropertyToID("_OutlineWidth");
        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        private FirTreesController _firTreesController;
        
        public void Initialize(FirTreesController firTreesController, string identifier)
        {
            _identifier = identifier;
            _materialVariant = GetComponent<MeshRenderer>().material;
            Debug.Log($"Tree matiral {_materialVariant}");
            _firTreesController = firTreesController;
        }

        public void Interact()
        {
            _firTreesController.Interactive(_identifier);
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
    }
}
