using System.Collections.Generic;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Inventory
{
    public sealed class InventoryController
    {
        private readonly Dictionary<string, int> _items = new ();
        private readonly Dictionary<string, int> _inStockHandheldObjects = new();
        
        public Dictionary<string, int> InStockHandheldObjects => _inStockHandheldObjects;

        public InventoryController()
        {
            AddHandObject("garden_bed", 1);
        }
        
        public int GetItemCount(string itemId)
        {
            if (_items.TryGetValue(itemId, out var value))
            {
                return value;
            }

            _items.Add(itemId, 0);
            return 0;
        }
        
        public void AddItem(string itemId, int count)
        {
            if (!_items.TryAdd(itemId, count))
            {
                _items[itemId] += count; 
                Debug.Log(_items[itemId]);
                return;
            }

            Debug.Log(_items[itemId]);
        }
        
        public void AddHandObject(string objectId, int count)
        {
            if (!_inStockHandheldObjects.TryAdd(objectId, count))
            {
                _inStockHandheldObjects[objectId] += count; 
            }
        }
    }
}