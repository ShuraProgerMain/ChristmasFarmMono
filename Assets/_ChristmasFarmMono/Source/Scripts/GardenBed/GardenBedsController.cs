using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using _ChristmasFarmMono.Source.Scripts.Items;
using UnityEngine;
using UnityTimer;

namespace _ChristmasFarmMono.Source.Scripts.GardenBed
{
    public enum InteractiveState
    {
        Input,
        Production,
        Output
    }

    public struct GardenBedState
    {
        public string Identifier;
        public InteractiveState InteractiveState;
    }
    
    public sealed class GardenBedsController : MonoBehaviour
    {
        [SerializeField] private GardenBedMediator[] gardenBeds = new GardenBedMediator[6];
        [SerializeField] private AnyIdentifier[] itemsForPlanting;

        private Dictionary<string, GardenBedState> _gardenBedStates;

        private GardenBedInput _gardenBedInput;
        private GardenBedProduction _gardenBedProduction;
        
        private void Awake()
        {
            _gardenBedStates = new Dictionary<string, GardenBedState>();
            
            foreach (var bed in gardenBeds)
            {
                bed.Initialize(this);
                _gardenBedStates.Add(bed.Identifier, 
                    new GardenBedState { Identifier = bed.Identifier, InteractiveState = InteractiveState.Input });
            }


            _gardenBedInput = new GardenBedInput
                (itemsForPlanting?.Select(x => x.ToString()).ToArray());
            _gardenBedProduction = new GardenBedProduction();
        }

        public void Interactive(string identifier)
        {
            if (!_gardenBedStates.TryGetValue(identifier, out var gardenBedState)) return;

            Debug.Log(gardenBedState.InteractiveState);
            switch (gardenBedState.InteractiveState)
            {
                case InteractiveState.Input:
                    var result = _gardenBedInput.ShowInput();
                    if (result.PlantingItems?.Count > 0)
                    {
                        _gardenBedStates[identifier] = new GardenBedState
                        {
                            Identifier = identifier,
                            InteractiveState = InteractiveState.Production
                        };
                        
                        _gardenBedProduction.SetProduction(identifier, result.PlantingItems);
                    }
                    break;
                case InteractiveState.Production:
                    break;
                case InteractiveState.Output:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public struct GardenBedInputResult
    {
        public List<string> PlantingItems;
    }
    
    public sealed class GardenBedInput
    {
        private readonly string[] _itemsForPlanting;

        private readonly List<string> _plantingItems = new(); // Вот эту штуку точно не стоит сохранять

        public GardenBedInput(string[] itemsForPlanting)
        {
            _itemsForPlanting = itemsForPlanting;
        }
        
        public GardenBedInputResult ShowInput()
        {
            var sb = new StringBuilder();

            foreach (var anyIdentifier in _itemsForPlanting)
            {
                sb.Append($"| {anyIdentifier} ");
            }

            Debug.Log(sb.ToString());
            SetPlantingItems();

            return new GardenBedInputResult(){ PlantingItems = _plantingItems };
        }

        private void SetPlantingItems()
        {
            _plantingItems.Add(_itemsForPlanting[^1]);
        }
    }

    public sealed class GardenBedProduction
    {
        public void SetProduction(string gardenBedId, List<string> productionItems)
        {
            var sb = new StringBuilder();

            foreach (var anyIdentifier in productionItems)
            {
                sb.Append($"| {anyIdentifier} ");
            }

            Debug.Log(sb + $"in garden: {gardenBedId}");

            Timer.Register(10f,
                () => Debug.Log($"Production id end in garden {gardenBedId}"));
        }
    }
}