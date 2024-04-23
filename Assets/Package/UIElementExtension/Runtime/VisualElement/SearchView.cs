using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public abstract class SearchView<TKey, TValue> : VisualElement
    {
        protected abstract IEnumerable<TValue> GetOrderedItem(TKey searchKey);
        protected abstract VisualElement CreateItemVisual(TValue value);

        protected Dictionary<TValue, VisualElement> itemVisuals = new();
        public int MaxDisplayCount = 20;

        public void Search(TKey key)
        {
            Clear();
            int i = 0;
            foreach (var item in GetOrderedItem(key))
            {
                if (i++ > MaxDisplayCount)
                    break;
                Add(GetOrCreateItemVisual(item));
            }
        }
        protected VisualElement GetOrCreateItemVisual(TValue key)
        {
            if (itemVisuals.TryGetValue(key, out var visual))
                return visual;
            visual = CreateItemVisual(key);
            itemVisuals.Add(key, visual);
            return visual;
        }
    }
}
