using System;
using System.Collections.Generic;
using System.Linq;
using _ChristmasFarmMono.Source.Scripts.Configs;
using _ChristmasFarmMono.Source.Scripts.Inventory;
using _ChristmasFarmMono.Source.Scripts.Items;
using _ChristmasFarmMono.Source.Scripts.Player;
using _ChristmasFarmMono.Source.Scripts.UI;
using JetBrains.Annotations;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _ChristmasFarmMono.Source.Scripts.InHandObjects.InHandsObjectsInventory
{
    public abstract class InHandsObjectReference : ScriptableObject
    {
        [field: SerializeField] public AnyIdentifier Identifier { get; private set; }

        public abstract Type HandheldObjectType { get; }
    }

    public sealed class InHandsObjectInventoryController : MonoBehaviour
    {
        [SerializeField] [NotNull] private GardenBedItemConfig gardenBedItemConfig;
        [SerializeField] [NotNull] private HandledObjectViewConfig handledObjectViewConfig;
        [SerializeField] [NotNull] private InHandsObjectReference reference;
        
        private readonly Dictionary<string, IHandheldObject> _inHandheldObjects = new();
        private InventoryController _inventoryController;

        private InHandsObjectInventoryView _handsObjectInventoryView;
        private InputActionsService _inputActionsService;
        private HandledObjectView _handledObjectView;
        private string _tempSelectedObject;
        private string _cachedSelectedObject;

        private Action<IHandheldObject> _selectHandledObject;
        private GameConfigs _gameConfigs;

        private Dictionary<string, int> InStockHandheldObjects => _inventoryController.InStockHandheldObjects;

        private Dictionary<string, IHandheldObject> InHandheldObjects => _inHandheldObjects;
        
        [Inject]
        public void Construct(InventoryController inventoryController, InputActionsService inputActionsService, GameConfigs gameConfigs, IObjectResolver resolver, InGameUIManager inGameUIManager)
        {
            _gameConfigs = gameConfigs;
            
            _handledObjectView = new HandledObjectView(handledObjectViewConfig);
            AddToDictionary(reference, resolver);
            _inputActionsService = inputActionsService;
            _inventoryController = inventoryController;
            _handsObjectInventoryView = new InHandsObjectInventoryView(inGameUIManager);

            _inputActionsService.GameplayActions.Character.InHandInventory.performed += _ =>
            {
                _handsObjectInventoryView.ShowView(InStockHandheldObjects, OnItemMouseDown, OnOkMouseDown);
                if (!string.IsNullOrEmpty(_cachedSelectedObject))
                {
                    _handsObjectInventoryView.SelectItem(_cachedSelectedObject);
                }
            };
        }

        public void Initialize(Action<IHandheldObject> selectHandledObject)
        {
            _selectHandledObject = selectHandledObject;
        }

        private void OnItemMouseDown(string itemId)
        {
            if (itemId == _tempSelectedObject)
            {
                _handsObjectInventoryView.UnselectItem(itemId);
                _tempSelectedObject = string.Empty;
                return;
            }

            if (!string.IsNullOrEmpty(_tempSelectedObject))
            {
                _handsObjectInventoryView.UnselectItem(_tempSelectedObject);
            }
            
            _tempSelectedObject = itemId;
            
            _handsObjectInventoryView.SelectItem(itemId);
        }

        private void OnOkMouseDown()
        {
            if (!string.IsNullOrEmpty(_tempSelectedObject))
            {
                _cachedSelectedObject = _tempSelectedObject;
                _selectHandledObject?.Invoke(InHandheldObjects[_tempSelectedObject]);
            }
            else
            {
                if (string.IsNullOrEmpty(_cachedSelectedObject)) return;
                
                InHandheldObjects[_cachedSelectedObject].HideCellVisualization(); 
                _cachedSelectedObject = string.Empty; 
                _selectHandledObject?.Invoke(null);
            }
        }
        
        private void AddToDictionary(InHandsObjectReference target, IObjectResolver resolver)
        {
            if (Activator.CreateInstance(target.HandheldObjectType) is not IHandheldObject instance)
                throw new NullReferenceException("Something went wrong");
            
            resolver.Inject(instance);
            instance.Initialize(_handledObjectView, _gameConfigs);
            _inHandheldObjects.Add(target.Identifier.Id, instance);
        }
    }

    public sealed class InHandsObjectInventoryView
    {
        private readonly ItemsHolderShow _itemsHolder;
        private readonly InGameUIManager _inGameUIManager;

        public InHandsObjectInventoryView(InGameUIManager inGameUIManager)
        {
            _inGameUIManager = inGameUIManager;
            _itemsHolder = _inGameUIManager.PrepareItemHolderForShow();
        }

        public void ShowView(IReadOnlyDictionary<string, int> inStockHandheldObjects, Action<string> onItemMouseDown, Action onOkMouseDown)
        {
            _itemsHolder.ShowItemsHolder(inStockHandheldObjects.Keys.ToList().AsReadOnly(), 
                string.Empty, 
                null, 
                onItemMouseDown, 
                onOkMouseDown);

            foreach (KeyValuePair<string, int> inStockHandheldObject in inStockHandheldObjects)
            {
                _itemsHolder.UpdateItemText(inStockHandheldObject.Key, inStockHandheldObject.Value.ToString());
            }
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