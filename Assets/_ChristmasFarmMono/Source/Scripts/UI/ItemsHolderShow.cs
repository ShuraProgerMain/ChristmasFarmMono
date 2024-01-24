#nullable enable
using System;
using System.Collections.Generic;
using _ChristmasFarmMono.Source.Scripts.InHandObjects.InHandsObjectsInventory;
using _ChristmasFarmMono.Source.Scripts.Inventory;
using _ChristmasFarmMono.Source.Scripts.ItemsDatabases;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

namespace _ChristmasFarmMono.Source.Scripts.UI
{
    public sealed class ItemsHolderShow : MonoBehaviour
    {
        [SerializeField] private UIDocument uiDocument;
        [SerializeField] private VisualTreeAsset itemCell;

        private VisualElement _container;
        private VisualElement _itemsHolder;

        private Action _onOkMouseDown;
        private ItemsViewUIDatabase _itemsViewUIDatabase;
        private readonly Dictionary<string, TemplateContainer> _instantiatedCells = new ();
        private InHandsObjectInventoryController _inHandsObjectInventoryController;

        private readonly ItemCellViewParameters _defaultCellParameters = new()
        {
            ItemSpriteSize = new int2(50, 50),
            ItemFontSize = 10
        };
        
        [Inject]
        private void Constructor(ItemsViewUIDatabase itemsViewUIDatabase, InventoryController inventoryController)
        {
            _itemsViewUIDatabase = itemsViewUIDatabase;
        }
        
        private void OnEnable()
        {
            _container = uiDocument.rootVisualElement.Q<VisualElement>("Container");
            _container.Q<VisualElement>("Button").RegisterCallback<MouseDownEvent>(OnOkButtonDown);
            _itemsHolder = uiDocument.rootVisualElement.Q<VisualElement>("ItemsHolder");
        }

        public void ShowItemsHolder(IReadOnlyList<string>? itemsIds, string textValue, 
            ItemCellViewParameters? viewParameters, Action<string> onItemMouseDown, Action onOkMouseDown)
        {
            if (itemsIds is null)
            {
                throw new ArgumentException($"Null readonly list ItemsIds in {nameof(ShowItemsHolder)}");
            }
            
            ClearHolder();
            
            _onOkMouseDown = onOkMouseDown;
            foreach (var itemsId in itemsIds)
            {
                var cell = GetCellInstance(_itemsViewUIDatabase?.GetSprite(itemsId)!, textValue, viewParameters);
                _instantiatedCells.Add(itemsId, cell); 
                cell.RegisterCallback<MouseDownEvent>(_ => onItemMouseDown?.Invoke(itemsId));
            }

            _container.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        public void HideItemsHolder()
        {
            ClearHolder();
        }

        public void UpdateItemText(string itemId, string text)
        {
            _instantiatedCells[itemId].Q<Label>("Count").text = text;
        }

        public void SelectItem(string itemIds)
        {
            var cell = _instantiatedCells[itemIds];
            cell.Q<Label>("Count").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            cell.Q<VisualElement>("Check").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }
        
        public void UnselectItem(string itemIds)
        {
            var cell = _instantiatedCells[itemIds];
            cell.Q<Label>("Count").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            cell.Q<VisualElement>("Check").style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        private void OnOkButtonDown(MouseDownEvent mouseDownEvent)
        {
            _onOkMouseDown?.Invoke();
            ClearHolder();
        }

        private TemplateContainer GetCellInstance(Sprite sprite, string text, ItemCellViewParameters? viewParameters)
        {
            viewParameters ??= _defaultCellParameters;
            
            var cell = itemCell.Instantiate();
            var cellSprite = cell.Q<VisualElement>("Sprite");
            var cellCount = cell.Q<Label>("Count");
            

            cellSprite.style.backgroundImage = new StyleBackground(sprite);
            cellSprite.style.width = new StyleLength(viewParameters.Value.ItemSpriteSize.x);
            cellSprite.style.maxWidth = new StyleLength(viewParameters.Value.ItemSpriteSize.x);
            cellSprite.style.height = new StyleLength(viewParameters.Value.ItemSpriteSize.y);
            cellSprite.style.maxHeight = new StyleLength(viewParameters.Value.ItemSpriteSize.y);
            cellCount.text = text;
            cellCount.style.fontSize = new StyleLength(viewParameters.Value.ItemFontSize);
            
            _itemsHolder.Add(cell);

            return cell;
        }

        private void ClearHolder()
        {
            _itemsHolder.Clear();
            _instantiatedCells.Clear();
            _container.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
    }
}
