using System.Threading.Tasks;
using _ChristmasFarmMono.Source.Scripts.ItemsDatabases;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _ChristmasFarmMono.Source.Scripts.UI
{
    public sealed class ItemsHolderShow : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset itemCell;

        private VisualElement _itemsHolder;

        private ItemsViewUIDatabase _itemsViewUIDatabase;
        
        [Inject]
        private void Constructor(ItemsViewUIDatabase itemsViewUIDatabase)
        {
            _itemsViewUIDatabase = itemsViewUIDatabase;
        }
        
        private void OnEnable()
        {
            _itemsHolder = uiDocument.rootVisualElement.Q<VisualElement>("ItemsHolder");
        }

        private async void Start()
        {
            await Task.Delay(2000);
            ShowItemsHolder();
        }

        public void ShowItemsHolder()
        {
            for (int i = 0; i < 2; i++)
            {
                SetCell(_itemsViewUIDatabase.GetSprite(0), $"{i} / 10");
            }
        }

        private void SetCell(Sprite sprite, string text)
        {
            var cell = itemCell.Instantiate();
            var cellSprite = cell.Q<VisualElement>("Sprite");
            var cellCount = cell.Q<Label>("Count");

            cellSprite.style.backgroundImage = new StyleBackground(sprite);
            cellCount.text = text;
            
            _itemsHolder.Add(cell);
        }
    }
}
