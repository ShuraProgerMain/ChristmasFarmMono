#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using _ChristmasFarmMono.Source.Scripts.ItemsDatabases;
using _ChristmasFarmMono.Source.Scripts.Player;
using AddressableExtensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace _ChristmasFarmMono.Source.Scripts.UI
{
    public sealed class ItemsHolderShow : UIPanelBase
    {
        private readonly VisualTreeAsset _itemCell;

        private VisualElement _container;
        private VisualElement _itemsContainer;

        private readonly ItemsViewUIDatabase _itemsViewUIDatabase;
        private readonly Dictionary<string, VisualElement> _instantiatedCells = new ();

        private readonly ItemCellViewParameters _defaultCellParameters = new()
        {
            ItemSpriteSize = new int2(50, 50),
            ItemFontSize = 10
        };
        
        private Action _onOkMouseDown;

        public ItemsHolderShow(ItemsViewUIDatabase itemsViewUIDatabase, InputActionsService inputActionsService) : base(inputActionsService)
        {
            _itemCell = Addressables.LoadAssetAsync<VisualTreeAsset>(UITemplates.ItemCell).WaitForCompletion();
            _itemsViewUIDatabase = itemsViewUIDatabase;
            
            inputActionsService.GameplayActions.UI.Click.performed += OnSelectFocused;
        }

        public override UIPanelBase Init(VisualElement topElement)
        {
            base.Init(topElement);
            
            _container = TopElement.Q<VisualElement>("Container");
            _itemsContainer = _container.Q<VisualElement>("AllItems");
            _container.Q<VisualElement>("Button").RegisterCallback<ClickEvent>(OnOkButtonDown);

            return this;
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
                cell.tabIndex = _instantiatedCells.Count + 1;
                _instantiatedCells.Add(itemsId, cell); 
                cell.RegisterCallback<ClickEvent>(_ => onItemMouseDown?.Invoke(itemsId));
            }
            
            ShowPanel();
            TopElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            
            _instantiatedCells.First().Value.Children().First().Focus();
        }

        public override void HidePanel()
        {
            ClearHolder();
            base.HidePanel();
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

        private void OnOkButtonDown(ClickEvent mouseDownEvent)
        {
            _onOkMouseDown?.Invoke();
            ClearHolder();
            base.HidePanel();
        }

        private void OnSelectFocused(InputAction.CallbackContext callbackContext)
        {
            SimulateClick(TopElement.focusController.focusedElement);
        }

        private VisualElement GetCellInstance(Sprite sprite, string text, ItemCellViewParameters? viewParameters)
        {
            viewParameters ??= _defaultCellParameters;
            
            var cell = _itemCell.CloneTree();
            var cellSprite = cell.Q<VisualElement>("Sprite");
            var cellCount = cell.Q<Label>("Count");


            cellSprite.style.backgroundImage = new StyleBackground(sprite);
            cellSprite.style.width = new StyleLength(viewParameters.Value.ItemSpriteSize.x);
            cellSprite.style.maxWidth = new StyleLength(viewParameters.Value.ItemSpriteSize.x);
            cellSprite.style.height = new StyleLength(viewParameters.Value.ItemSpriteSize.y);
            cellSprite.style.maxHeight = new StyleLength(viewParameters.Value.ItemSpriteSize.y);
            cellCount.text = text;
            cellCount.style.fontSize = new StyleLength(viewParameters.Value.ItemFontSize);
            
            _itemsContainer.Add(cell);

            return cell;
        }

        private void ClearHolder()
        {
            _itemsContainer.Clear();
            _instantiatedCells.Clear();
            TopElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }
    }
}
