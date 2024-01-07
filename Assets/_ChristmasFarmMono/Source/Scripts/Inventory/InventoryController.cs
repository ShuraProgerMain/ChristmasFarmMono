using System.Collections.Generic;
using UnityEngine;

namespace _ChristmasFarmMono.Source.Scripts.Inventory
{
    public sealed class InventoryController
    {
        private readonly Dictionary<string, int> _items = new ();

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
    }
}