using System;
using System.Collections.Generic;
using System.Linq;
using _ChristmasFarmMono.Source.Scripts.InHandObjects;
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
    
    public sealed class GardenBedsBehaviourManager : MonoBehaviour
    {
        [SerializeField] private AnyIdentifier[] itemsForPlanting;

        [SerializeField] private AssetReferenceT<GardenBedsHarvestConfig> harvestConfigReference;
        
        private Dictionary<string, GardenBedState> _gardenBedStates;
        private Dictionary<string, int> _harvestAmounts;

        private GardenBedInput _gardenBedInput;
        private GardenBedProduction _gardenBedProduction;
        private GardenBedOutput _gardenBedOutput;
        private InventoryController _inventoryController;
        private GardenBedsSpawner _gardenBedsSpawner;

        private GardenBedsHarvestConfig _harvestConfig;

        [Inject]
        private void Construct(ItemsViewDatabase itemsViewDatabase, InventoryController inventoryController, 
            InGameUIManager inGameUIManager, GardenBedsSpawner gardenBedsSpawner)
        {
            _gardenBedStates = new Dictionary<string, GardenBedState>();
            _gardenBedsSpawner = gardenBedsSpawner;
            _gardenBedsSpawner.CreatedGardenBed += AddNewGardenBed;
            
            _harvestConfig = harvestConfigReference.LoadAssetAsync().WaitForCompletion();

            _harvestAmounts = _harvestConfig.HarvestAmounts.ToDictionary(k => k.First.Id, 
                v => v.Second);
            
            // Change to states
            _gardenBedInput = new GardenBedInput(itemsForPlanting?.Select(x => x.Id).ToArray(), inGameUIManager);
            _gardenBedProduction = new GardenBedProduction(inGameUIManager, itemsViewDatabase);
            _gardenBedOutput = new GardenBedOutput(inGameUIManager);
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

        private void AddNewGardenBed(GardenBedMediator gardenBed)
        {
            var identifier = _harvestConfig.DefaultGardenBedIdentifier.Id + _gardenBedStates.Count;

            gardenBed.Initialize(this, identifier);
            
            _gardenBedStates.Add(gardenBed.Identifier, new GardenBedState() { Identifier = gardenBed.Identifier, InteractiveState = InteractiveState.Input });
        }

        #region InteractiveResults

        
        private void InteractiveOutputResult(GardenBedOutputResult result)
        {
            _inventoryController.AddItem(result.OutputItemId, 5);

            _gardenBedStates[result.GardenBedId].InteractiveState = InteractiveState.Input;
            
            _gardenBedsSpawner.GetGardenBedMediator(result.GardenBedId).ClearGardenBed();
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
            
            _gardenBedStates[result.GardenBedId].InteractiveState = InteractiveState.Production;

            Debug.Log(result.ToString());
            
            _gardenBedProduction.SetProduction(_gardenBedsSpawner.GetGardenBedMediator(result.GardenBedId), result.GardenBedId, result.PlantingItem, InteractiveProductionResult);
        }

        #endregion
    }
}