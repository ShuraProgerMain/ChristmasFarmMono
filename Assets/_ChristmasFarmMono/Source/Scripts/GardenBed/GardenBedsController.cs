using System;
using System.Collections.Generic;
using System.Linq;
using _ChristmasFarmMono.Source.Scripts.Inventory;
using _ChristmasFarmMono.Source.Scripts.Items;
using _ChristmasFarmMono.Source.Scripts.ItemsDatabases;
using _ChristmasFarmMono.Source.Scripts.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public enum InteractiveState
    {
        Input,
        Production,
        Output
    }

    public sealed class GardenBedState
    {
        public string Identifier;
        public InteractiveState InteractiveState;
    }
    
    public sealed class GardenBedsController : MonoBehaviour
    {
        [SerializeField] private GardenBedMediator[] gardenBeds = new GardenBedMediator[6];
        [SerializeField] private AnyIdentifier[] itemsForPlanting;

        [SerializeField] private ItemsHolderShow itemsHolderView;
        [SerializeField] private AssetReferenceT<GardenBedsHarvestConfig> harvestConfigReference;
        
        private Dictionary<string, GardenBedState> _gardenBedStates;
        private Dictionary<string, int> _harvestAmounts;

        private GardenBedInput _gardenBedInput;
        private GardenBedProduction _gardenBedProduction;
        private GardenBedOutput _gardenBedOutput;
        private InventoryController _inventoryController;

        [Inject]
        private void Construct(ItemsViewDatabase itemsViewDatabase, InventoryController inventoryController)
        {
            _gardenBedStates = new Dictionary<string, GardenBedState>();
            
            foreach (var bed in gardenBeds)
            {
                bed.Initialize(this);
                _gardenBedStates.Add(bed.Identifier, 
                    new GardenBedState { Identifier = bed.Identifier, InteractiveState = InteractiveState.Input });
            }
            
            var harvestConfig = harvestConfigReference.LoadAssetAsync().WaitForCompletion();

            _harvestAmounts = harvestConfig.HarvestAmounts.ToDictionary(k => k.First.Id, 
                v => v.Second);
            
            foreach (var keyValuePair in _harvestAmounts)
            {
                Debug.Log($"This key: {keyValuePair.Key} and value: {keyValuePair.Value}");
            }
            
            _gardenBedInput = new GardenBedInput(itemsForPlanting?.Select(x => x.Id).ToArray(), itemsHolderView);
            _gardenBedProduction = new GardenBedProduction(itemsHolderView, itemsViewDatabase);
            _gardenBedOutput = new GardenBedOutput(itemsHolderView);
            _inventoryController = inventoryController;
        }
        
        public void Interactive(string identifier)
        {
            if (!_gardenBedStates.TryGetValue(identifier, out var gardenBedState)) return;

            Debug.Log(gardenBedState.InteractiveState);
            switch (gardenBedState.InteractiveState)
            {
                case InteractiveState.Input:
                    _gardenBedInput.ShowInput(identifier, InteractiveInputResult);
                    break;
                case InteractiveState.Production:
                    _gardenBedProduction.ShowProduction(identifier);
                    break;
                case InteractiveState.Output:
                    _gardenBedOutput.ShowOutput(identifier, InteractiveOutputResult);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region InteractiveResults

        
        private void InteractiveOutputResult(GardenBedOutputResult result)
        {
            _inventoryController.AddItem(result.OutputItemId, 5);

            _gardenBedStates[result.OutputGardenBedId].InteractiveState = InteractiveState.Input;
            
            gardenBeds.First(x => x.Identifier == result.OutputGardenBedId).ClearGardenBed();
        }

        private void InteractiveProductionResult(ProductionResult result)
        {
            _gardenBedStates[result.ProductionGardenBedId].InteractiveState = InteractiveState.Output;
            
            _gardenBedOutput.SetOutput(result.ProductionGardenBedId, result.ProductionItemId);

            if (result.ProductionIsShowed)
                _gardenBedOutput.ShowOutput(result.ProductionGardenBedId, InteractiveOutputResult);
        }

        private void InteractiveInputResult(GardenBedInputResult result)
        {
            if (string.IsNullOrWhiteSpace(result.PlantingItem))
                return;

            _gardenBedStates[result.CurrentGardenBed].InteractiveState = InteractiveState.Production;
            
            _gardenBedProduction.SetProduction(gardenBeds
                    .First(x => x.Identifier == result.CurrentGardenBed),
                result.CurrentGardenBed, result.PlantingItem, InteractiveProductionResult);
        }

        #endregion
    }
}