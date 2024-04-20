using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class PopupElement : VisualElement
    {
        public bool IsOpend => panel != null;
        public bool AutoClose = true;
        public bool LimitInMask = true;
        public event Action<VisualElement> OnOpend;
        public event Action OnClosed;
        public readonly VisualElement CoverMask;

        public PopupElement() : this(null, true) { }
        public PopupElement(bool autoClose) : this(null, autoClose) { }
        public PopupElement(VisualElement coverMask, bool autoClose)
        {
            if(coverMask == null)
            {
                CoverMask = new();
                CoverMask.name = "popup-mask";
                CoverMask.style.width = Length.Percent(100);
                CoverMask.style.height = Length.Percent(100);
                CoverMask.style.position = Position.Absolute;
            }
            else
            {
                CoverMask = coverMask;
            }
            CoverMask.Add(this);
            style.position = Position.Absolute;
            style.left = 0;
            style.top = 0;

            RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (!LimitInMask) return;
                if (panel == null) return;
                if((evt.newRect.position - evt.oldRect.position).magnitude > 1)
                    this.LimitPositionIn(CoverMask);
            });
            CoverMask.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (!AutoClose) return;
                if (worldBound.Contains(evt.position)) return;
                Close();
            });
            AutoClose = autoClose;
        }

        public void OpenBelow(VisualElement openFrom) { Open(openFrom, new Vector2(openFrom.worldBound.x, openFrom.worldBound.yMax)); }
        public void Open(VisualElement openFrom, Vector2 worldPosition)
        {
            if (openFrom.panel == null) return;
#if UNITY_EDITOR
            VisualElement root = openFrom.panel.visualTree[openFrom.panel.visualTree.childCount - 1];
#else
            VisualElement root = openFrom.panel.visualTree;
#endif
            root.Add(CoverMask);
            var pos = parent.WorldToLocal(worldPosition);
            style.top = pos.y;
            style.left = pos.x;
            OnOpend?.Invoke(openFrom);
        }
        public void Close()
        {
            if (CoverMask.parent != null)
            {
                CoverMask.parent.Remove(CoverMask);
                OnClosed?.Invoke();
            }
        }
    }
}
