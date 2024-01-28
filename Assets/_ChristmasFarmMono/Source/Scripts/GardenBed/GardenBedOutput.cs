using System;
using System.Collections.Generic;
using _ChristmasFarmMono.Source.Scripts.UI;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed record GardenBedOutputResult
    {
        public string GardenBedId { get; init; }
        public string OutputItemId { get; init; }
    }
    
    public sealed class GardenBedOutput
    {
        private readonly GardenBedOutputView _gardenBedOutputView;
        private readonly Dictionary<string, GardenBedOutputResult> _gardenBedOutputStates = new ();

        private GardenBedOutputResult _tempOutputResult;
        
        private Action<GardenBedOutputResult> _outputResult;

        public GardenBedOutput(InGameUIManager inGameUIManager)
        {
            _gardenBedOutputView = new GardenBedOutputView(inGameUIManager);
        }

        public void ShowOutput(string gardenBedId, Action<GardenBedOutputResult> outputResult)
        {
            _outputResult = outputResult;
            _tempOutputResult = _gardenBedOutputStates[gardenBedId];
            _gardenBedOutputView.ShowOutput(_tempOutputResult.OutputItemId, OnOkMouseDown);
        }

        private void OnOkMouseDown()
        {
            _gardenBedOutputStates.Remove(_tempOutputResult.GardenBedId);
            _outputResult?.Invoke(_tempOutputResult);
        }
        
        public void SetOutput(string gardenBedId, string itemId)
        {
            _gardenBedOutputStates.Add(gardenBedId,
                new GardenBedOutputResult
                {
                    GardenBedId = gardenBedId,
                    OutputItemId = itemId
                });
        }
    }

    public sealed class GardenBedOutputView
    {
        private readonly ItemsHolderShow _itemsHolderShow;
        
        public GardenBedOutputView(InGameUIManager inGameUIManager)
        {
            _itemsHolderShow = inGameUIManager.PrepareItemHolderForShow();
        }

        public void ShowOutput(string itemId, Action onOkMouseDown)
        {
            _itemsHolderShow.ShowItemsHolder(new []{ itemId }, "Complete 3", 
                null, null, onOkMouseDown);
        }
    }
}