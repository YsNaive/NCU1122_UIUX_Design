using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    public interface IFoldoutDrawer
    {
        public bool FoldoutState { get; set; }
    }
    public abstract class FoldoutDrawer<T> : RuntimeDrawer<T>, IFoldoutDrawer
    {
        public bool FoldoutState
        {
            get => contentContainer.style.display == DisplayStyle.Flex;
            set
            {
                contentContainer.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
                iconElement.transform.rotation = Quaternion.Euler(0, 0, value ? 90 : 0);
            }
        }
        public FoldoutDrawer()
        {
            LayoutExpand();
        }
        public override void LayoutExpand()
        {
            base.LayoutExpand();
            titleElement.style.SetIS_Style(new ISBorder(DocStyle.Current.BackgroundColor, 1f));
            titleElement.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
            iconElement.style.backgroundImage = new StyleBackground(DocStyle.Current.ArrowSprite);
            iconElement.style.opacity = 0.5f;
            FoldoutState = false;
            contentContainer.style.marginLeft = DocStyle.Current.LineHeight.Value;
            RegisterFoldoutStateCallback(titleElement);
        }
        public override void LayoutInline()
        {
            base.LayoutInline();
            titleElement.style.SetIS_Style(new ISBorder(Color.clear, 0f));
            titleElement.style.backgroundColor = Color.clear;
            iconElement.style.opacity = 0f;
            FoldoutState = true;
            contentContainer.style.marginLeft = 0;
            UnregisterFoldoutStateCallback(titleElement);
        }
        public void RegisterFoldoutStateCallback(VisualElement visual)
        {
            visual.RegisterCallback<PointerDownEvent>(PointerDownEvent);
            visual.RegisterCallback<PointerEnterEvent>(PointerEnterEvent);
            visual.RegisterCallback<PointerLeaveEvent>(PointerLeaveEvent);
        }
        public void UnregisterFoldoutStateCallback(VisualElement visual)
        {
            visual.UnregisterCallback<PointerDownEvent>(PointerDownEvent);
            visual.UnregisterCallback<PointerEnterEvent>(PointerEnterEvent);
            visual.UnregisterCallback<PointerLeaveEvent>(PointerLeaveEvent);
        }
        void PointerDownEvent(PointerDownEvent evt)
        {
            if (evt.pointerType == "mouse")
                if (evt.button != (int)MouseButton.LeftMouse) return;
            FoldoutState = !FoldoutState;
            evt.StopPropagation();
        }
        void PointerEnterEvent(PointerEnterEvent evt)
        {
            iconElement.style.opacity = 1f;
        }
        void PointerLeaveEvent(PointerLeaveEvent evt)
        {
            iconElement.style.opacity = .5f;
        }
    }
}