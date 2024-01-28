using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _ChristmasFarmMono.Source.Scripts.UI;
using Unity.Mathematics;
using UnityEngine;
using Timer = UnityTimer.Timer;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed class FirTreeProduction
    {
        private readonly Dictionary<string, ProductionState> _inProductionFirTrees = new();
        private readonly ItemsHolderShow _itemsHolderShow;
        
        private CancellationTokenSource _updateTimerSource;
        private Task _updateTimerTask;
        
        public FirTreeProduction(InGameUIManager inGameUIManager)
        {
            _itemsHolderShow = inGameUIManager.PrepareItemHolderForShow();
        }

        public void ShowProduction(string firTreeId)
        {
            var productionState = _inProductionFirTrees[firTreeId];

            if (productionState.ProductionEndTime > DateTime.UtcNow)
            {
                var viewParameters = new ItemCellViewParameters()
                {
                    ItemFontSize = 36,
                    ItemSpriteSize = new int2(150, 150)
                };
                
                _itemsHolderShow.ShowItemsHolder(new [] { productionState.ProductionItemId }, 
                    string.Empty, viewParameters, null, HideProduction);
                _updateTimerSource = new CancellationTokenSource();
                _updateTimerTask = StartUpdateTimer(productionState, _updateTimerSource.Token);
            }
        }
        
        public void SetProduction(string firTreeId, string itemId, Action<ProductionResult> productionComplete)
        {
            _inProductionFirTrees.Add(firTreeId, new ProductionState()
            {
                ProductionItemId = itemId,
                ProductionStartTime = DateTime.UtcNow,
                ProductionEndTime = DateTime.UtcNow.AddSeconds(20),
                ProductionComplete = productionComplete
            });

            Timer.Register(20, () => ProductionComplete(firTreeId));
        }

        private async void HideProduction()
        {
            try
            {
                if (_updateTimerTask is not null)
                {
                    _updateTimerSource.Cancel();
                    await _updateTimerTask;
                    _updateTimerTask = null;
                    _updateTimerSource.Dispose();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        
        private async Task StartUpdateTimer(ProductionState productionState, CancellationToken token)
        {
            await TimerView.UpdateEverySecond(() =>
            {
                UpdateTimerText(productionState.ProductionItemId, productionState.ProductionEndTime - DateTime.UtcNow);
            }, () => productionState.ProductionEndTime > DateTime.UtcNow, token);
            
            if (_updateTimerTask is not null)
            {
                HideProduction();
                _itemsHolderShow.HidePanel();
            }
        }

        private void UpdateTimerText(string itemId, TimeSpan currentTime)
        {
            _itemsHolderShow.UpdateItemText(itemId, currentTime.ToString(TextFormats.MinutesAndSeconds));
        }

        private void ProductionComplete(string firTreeId)
        {
            var productionState = _inProductionFirTrees[firTreeId];
            
            productionState.ProductionComplete?.Invoke(new ProductionResult()
            {
                ProductionItemId = productionState.ProductionItemId,
                ProductionGardenBedId = firTreeId,
            });

            _inProductionFirTrees.Remove(firTreeId);
        }
    }
}