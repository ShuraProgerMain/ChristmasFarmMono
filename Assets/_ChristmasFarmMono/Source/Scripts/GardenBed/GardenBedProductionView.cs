using System;
using _ChristmasFarmMono.Source.Scripts.UI;
using Unity.Mathematics;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed class GardenBedProductionView
    {
        private readonly ItemsHolderShow _itemsHolderShow;
        private const string MinutesAndSeconds = @"mm'm'\:ss's'";
        private readonly Action _onHideView;
        
        public GardenBedProductionView(InGameUIManager inGameUIManager, Action onHideView)
        {
            _itemsHolderShow = inGameUIManager.PrepareItemHolderForShow();
            _onHideView = onHideView;
        }

        public void UpdateTimerText(string itemId, TimeSpan currentTime)
        {
            _itemsHolderShow.UpdateItemText(itemId, currentTime.ToString(MinutesAndSeconds));
        }
        
        public void ShowView(ProductionState productionState)
        {
            var viewParameters = new ItemCellViewParameters()
            {
                ItemFontSize = 36,
                ItemSpriteSize = new int2(150, 150)
            };
            var t = productionState.ProductionEndTime - DateTime.UtcNow;
            _itemsHolderShow.ShowItemsHolder(new []{ productionState.ProductionItemId }, t.ToString(MinutesAndSeconds), 
                viewParameters, null, _onHideView);
        }
    }
}