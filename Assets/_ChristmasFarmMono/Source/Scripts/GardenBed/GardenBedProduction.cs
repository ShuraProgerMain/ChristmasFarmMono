using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using _ChristmasFarmMono.Source.Scripts.ItemsDatabases;
using _ChristmasFarmMono.Source.Scripts.UI;
using UnityEngine;
using Timer = UnityTimer.Timer;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public sealed class GardenBedProduction
    {
        private readonly ItemsViewDatabase _itemsViewDatabase;
        private readonly GardenBedProductionView _gardenBedProductionView;
        private readonly Dictionary<string, ProductionState> _inProductionGardenBeds = new();

        private Task _updateTimerTask;
        private CancellationTokenSource _updateTimerSource;

        private bool _productionIsShowed;
        
        public GardenBedProduction(ItemsHolderShow itemsHolderShow, 
            ItemsViewDatabase itemsViewDatabase)
        {
            _itemsViewDatabase = itemsViewDatabase;

            _gardenBedProductionView = new GardenBedProductionView(itemsHolderShow, OnHideView);
        }

        public void ShowProduction(string gardenBedIdentifier)
        {
            if (_updateTimerTask != null)
                return;
            
            _productionIsShowed = true;
            
            var productionState = _inProductionGardenBeds[gardenBedIdentifier];
            _gardenBedProductionView.ShowView(productionState);

            if (productionState.ProductionEndTime > DateTime.UtcNow)
            {
                _updateTimerSource = new CancellationTokenSource();
                _updateTimerTask = StartUpdateTimer(productionState, _updateTimerSource.Token);
            }
        }
        
        public void SetProduction(GardenBedMediator gardenBedMediator, string gardenBedId, 
            string productionItem, Action<ProductionResult> productionComplete)
        {
            if (_inProductionGardenBeds.ContainsKey(gardenBedId)) return;
            
            _inProductionGardenBeds.Add(
                gardenBedId,
                new ProductionState()
                {
                    ProductionItemId = productionItem,
                    ProductionStartTime = DateTime.UtcNow,
                    ProductionEndTime = DateTime.UtcNow.AddSeconds(10),
                    ProductionComplete = productionComplete
                });

            gardenBedMediator.SetItem(_itemsViewDatabase.GetGameObject(productionItem));
            Timer.Register(10f, () => ProductionComplete(gardenBedId));
        }

        private async Task StartUpdateTimer(ProductionState productionState, CancellationToken token)
        {
            try
            {
                while (productionState.ProductionEndTime > DateTime.UtcNow && !token.IsCancellationRequested)
                {
                    _gardenBedProductionView.UpdateTimerText(productionState.ProductionItemId, productionState.ProductionEndTime - DateTime.UtcNow);
                    await Task.Delay(1000);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private void ProductionComplete(string gardenBedId)
        {
            var productionState = _inProductionGardenBeds[gardenBedId];
            productionState.ProductionComplete?.Invoke(new ProductionResult()
            {
                ProductionItemId = productionState.ProductionItemId,
                ProductionGardenBedId = gardenBedId,
                ProductionIsShowed = _productionIsShowed
            });
            _inProductionGardenBeds.Remove(gardenBedId);
            _productionIsShowed = false;
            _updateTimerTask = null;
        }

        private async void OnHideView()
        {
            try
            {
                if (_updateTimerSource is not null)
                {
                    _updateTimerSource.Cancel();
                    await _updateTimerTask;
                    _updateTimerTask = null;
                    _updateTimerSource.Dispose();
                }
                
                _productionIsShowed = false;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }
    }
}