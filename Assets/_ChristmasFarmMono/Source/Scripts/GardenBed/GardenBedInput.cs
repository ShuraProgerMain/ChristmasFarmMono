using System;
using System.Collections.Generic;
using _ChristmasFarmMono.Source.Scripts.UI;
using UnityEngine.Serialization;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed class GardenBedInput
    {
        private readonly string[]? _itemsForPlanting;

        private readonly GardenBedInputView _gardenBedInputView;

        private GardenBedInputResult _tempResult;
        private Action<GardenBedInputResult> _onResultedInput;
        
        public GardenBedInput(string[]? itemsForPlanting, InGameUIManager inGameUIManager)
        {
            _itemsForPlanting = itemsForPlanting;
            _gardenBedInputView = new GardenBedInputView(inGameUIManager);
        }
        
        public void ShowInput(string currentGardenBedId, Action<GardenBedInputResult> onResultedInput)
        {
            _tempResult.GardenBedId = currentGardenBedId;
            _onResultedInput = onResultedInput;
            _gardenBedInputView.ShowItems(_itemsForPlanting, OnItemMouseDown, OnOkMouseDown);
        }

        private void OnItemMouseDown(string itemId)
        {
            if (_tempResult.PlantingItem == itemId)
            {
                _gardenBedInputView.UnselectItem(_tempResult.PlantingItem);
                _tempResult.PlantingItem = string.Empty;
                return;
            }

            if (!string.IsNullOrWhiteSpace(_tempResult.PlantingItem))
            {
                _gardenBedInputView.UnselectItem(_tempResult.PlantingItem);
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
        [FormerlySerializedAs("CurrentGardenBed")] public string GardenBedId;
        public string PlantingItem;

        public override string ToString()
        {
            return $"CurrentGardenBed: {GardenBedId}; \n" +
                   $"PlantingItem: {PlantingItem}";
        }
    }

    public sealed class GardenBedInputView
    {
        private readonly ItemsHolderShow _itemsHolder;
        
        public GardenBedInputView(InGameUIManager inGameUIManager)
        {
            _itemsHolder = inGameUIManager.PrepareItemHolderForShow();
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