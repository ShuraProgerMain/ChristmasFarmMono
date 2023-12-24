using System;
using System.Collections.Generic;
using _ChristmasFarmMono.Source.Scripts.GardenBed;
using _ChristmasFarmMono.Source.Scripts.Inventory;
using _ChristmasFarmMono.Source.Scripts.Items;
using UnityEngine;
using VContainer;
using Random = UnityEngine.Random;

namespace _ChristmasFarmMono.Source.Scripts.Forest.FirTree
{
    public sealed record FirTreeState
    {
        public string Identifier { get; init; }
        public InteractiveState InteractiveState;
        public FirTreeMediator Mediator { get; init; }
    }
    
    public class FirTreesController : MonoBehaviour
    {
        [SerializeField] private AnyIdentifier woodItemId;
        [SerializeField] private AnyIdentifier identifierTemplate;
        [SerializeField] private FirTreeMediator[] firTrees;

        private readonly Dictionary<string, FirTreeState> _firTreesStates = new ();
        
        private InventoryController _inventoryController;
        private FirTreeInput _firTreeInput;

        [Inject]
        private void Construct(InventoryController inventoryController)
        {
            _inventoryController = inventoryController;
            
            for (var i = 0; i < firTrees.Length; i++)
            {
                var id = identifierTemplate.Id + i;
                firTrees[i].Initialize(this, id);
                _firTreesStates.Add(id, new FirTreeState
                {
                    Identifier = id, 
                    InteractiveState = InteractiveState.Input,
                    Mediator = firTrees[i]
                });
            }

            _firTreeInput = new FirTreeInput();
        }

        // Funk<Task, PlayerInteractiveActions>?
        // Funk<Task> middleAction? but if i want to get some info?
        // InteractiveActionKey = ActionId(AnyIdentifier)?
        public void Interactive(string identifier)
        {
            var firState = _firTreesStates[identifier];

            switch (firState.InteractiveState)
            {
                case InteractiveState.Input:
                    
                    _firTreeInput.SetInput(() =>
                    {
                        FirTreeInputResult(identifier);
                    });
                    
                    break;
                case InteractiveState.Production:
                    break;
                case InteractiveState.Output:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void FirTreeInputResult(string firId)
        {
            _inventoryController.AddItem(woodItemId.Id, Random.Range(2, 6));
            
            _firTreesStates[firId].InteractiveState = InteractiveState.Production;
            
            Debug.Log(" Zaconchilas suetaaaa ");
        }
    }
}