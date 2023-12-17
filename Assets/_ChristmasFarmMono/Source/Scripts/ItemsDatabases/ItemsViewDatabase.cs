using System.Collections.Generic;
using System.Linq;
using _ChristmasFarmMono.Source.Scripts.Items;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace _ChristmasFarmMono.Source.Scripts.ItemsDatabases
{
    public sealed class ItemsViewDatabase
    {
        private Dictionary<string, ItemView> _itemViews;

        public ItemsViewDatabase()
        {
            Addressables.LoadAssetsAsync<ItemView>("ItemViews", o =>
            {
                Debug.Log(o.name);
            }).Completed += handle =>
            {
                _itemViews = handle.Result.ToDictionary(x => x.Identifier);
            };
        }
    }

    public sealed class ItemsViewUIDatabase
    {
        private Dictionary<string, ItemViewUI> _itemViewUis;

        public ItemsViewUIDatabase()
        {
            Addressables.LoadAssetsAsync<ItemViewUI>("ItemViews", o =>
            {
                Debug.Log(o.name);
            }).Completed += handle =>
            {
                _itemViewUis = handle.Result.ToDictionary(x => x.Identifier);
            };
        }

        public Sprite GetSprite(int index)
        {
            return _itemViewUis.First().Value.ItemSprite;
        }
    }
}