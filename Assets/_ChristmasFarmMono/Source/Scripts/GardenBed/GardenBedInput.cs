using System;
using System.Collections.Generic;
using _ChristmasFarmMono.Source.Scripts.UI;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed class GardenBedInput
    {
        private readonly string[]? _itemsForPlanting;

        private readonly GardenBedInputView _gardenBedInputView;

        private GardenBedInputResult _tempResult = new();
        private Action<GardenBedInputResult> _onResultedInput;
        
        public GardenBedInput(string[]? itemsForPlanting, ItemsHolderShow itemsHolderShow)
        {
            _itemsForPlanting = itemsForPlanting;
            _gardenBedInputView = new GardenBedInputView(itemsHolderShow);
        }
        
        public void ShowInput(string currentGardenBedId, Action<GardenBedInputResult> onResultedInput)
        {
            _tempResult.CurrentGardenBed = currentGardenBedId;
            _onResultedInput = onResultedInput;
            _gardenBedInputView.ShowItems(_itemsForPlanting, OnItemMouseDown, OnOkMouseDown);
        }

        private void OnItemMouseDown(string itemId)
        {
            if (_tempResult.PlantingItem == itemId || !string.IsNullOrWhiteSpace(_tempResult.PlantingItem))
            {
                _gardenBedInputView.UnselectItem(_tempResult.PlantingItem);
                _tempResult.PlantingItem = string.Empty;
            }

            _tempResult.PlantingItem = itemId;
            _gardenBedInputView.SelectItem(itemId);
        }

        private void OnOkMouseDown()
        {
            if (string.IsNullOrWhiteSpace(_tempResult.PlantingItem))
                return;
            
            _onResultedInput?.Invoke(_tempResult);
            _tempResult = new GardenBedInputResult();
        }
    }
    
    public struct GardenBedInputResult
    {
        public string CurrentGardenBed;
        public string PlantingItem;
    }

    public sealed class GardenBedInputView
    {
        private readonly ItemsHolderShow _itemsHolder;
        
        public GardenBedInputView(ItemsHolderShow itemsHolder)
        {
            _itemsHolder = itemsHolder;
        }

        public void ShowItems(IReadOnlyList<string>? itemsIds, Action<string> onItemMouseDown, Action onOkMouseDown)
        {
            _itemsHolder.ShowItemsHolder(itemsIds, "0", null, onItemMouseDown, onOkMouseDown);
        }

        public void SelectItem(string itemIds)
        {
            _itemsHolder.SelectItem(itemIds);
        }
        
        public void UnselectItem(string itemIds)
        {
            _itemsHolder.UnselectItem(itemIds);
        }
    }
}