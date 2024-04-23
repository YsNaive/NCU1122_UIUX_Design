using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class PageView<TKey> : VisualElement
    {
        public override VisualElement contentContainer => currentContainer;
        private VisualElement currentContainer = null;

        public readonly Dictionary<TKey, VisualElement> pageTable = new();
        public PageView() { }
        public PageView(TKey initPageKey)
        {
            OpenOrCreatePage(initPageKey);
        }

        public bool TryOpenPage(TKey key)
        {
            if (pageTable.TryGetValue(key, out var ve))
            {
                SetDisplay(ve);
                return true;
            }
            return false;
        }
        public void OpenPage(TKey key)
        {
            SetDisplay(pageTable[key]);
        }
        public void OpenOrCreatePage(TKey key)
        {
            SetDisplay(pageTable.GetOrCreateValue(key));
        }
        public void CreatePage(TKey key, VisualElement container)
        {
            pageTable.SetOrCreateKey(key, container);
        }
        public void SetDisplay(VisualElement element)
        {
            currentContainer = element;
            hierarchy.Clear();
            hierarchy.Add(currentContainer);
        }
    }
}
