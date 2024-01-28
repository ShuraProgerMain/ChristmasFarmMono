using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _ChristmasFarmMono.Source.Scripts.UI
{
    public sealed class InGameUIManager : MonoBehaviour
    {
        [SerializeField] [NotNull] private UIDocument rootUIDocument;

        private ItemsHolderShow _itemsHolderShow;
        
        private const string ItemHolderName = "ItemsHolder";
        
        [Inject]
        private void Construct(ItemsHolderShow itemsHolderShow)
        {
            _itemsHolderShow = (ItemsHolderShow)itemsHolderShow.Init(rootUIDocument.rootVisualElement.Q<VisualElement>(ItemHolderName));
        }

        public ItemsHolderShow PrepareItemHolderForShow()
        {
            return _itemsHolderShow;
        }
    }
}