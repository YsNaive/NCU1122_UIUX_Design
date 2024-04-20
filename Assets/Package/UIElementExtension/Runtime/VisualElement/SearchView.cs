using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class SearchView<TKey, TValue> : VisualElement
    {
        protected Func<TKey, IEnumerable<TValue>> GetOrderedItem;
        protected Func<TValue, VisualElement> CreateItemVisual;

        protected Dictionary<TValue, VisualElement> itemVisuals = new();
        public readonly RSScrollView itemContainer;
        public int MaxDisplayCount = 20;
        public SearchView(Func<TKey, IEnumerable<TValue>> getOrderedItem, Func<TValue, VisualElement> createItemVisual)
        {
            GetOrderedItem = getOrderedItem;
            CreateItemVisual = createItemVisual;
            itemContainer = new();
            Add(itemContainer);
        }

        public void Search(TKey key)
        {
            itemContainer.Clear();
            int i = 0;
            foreach (var item in GetOrderedItem(key))
            {
                if (i++ > MaxDisplayCount)
                    break;
                itemContainer.Add(GetOrCreateItemVisual(item));
            }
        }
        protected VisualElement GetOrCreateItemVisual(TValue key)
        {
            if(itemVisuals.TryGetValue(key, out var visual))
                return visual;
            visual = CreateItemVisual(key);
            itemVisuals.Add(key, visual);
            return visual;
        }
    }
}
