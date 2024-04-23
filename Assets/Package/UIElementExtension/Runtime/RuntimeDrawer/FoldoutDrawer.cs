using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public interface IFoldoutDrawer
    {
        public bool FoldoutState { get; set; }
        public void DisFoldout();
        public void DisFoldoutRecursive();
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
        public bool IsLayoutExpand => m_IsLayoutExpand;
        bool m_IsLayoutExpand = false;
        public VisualElement iconElement2;
        public FoldoutDrawer()
        {
            labelElement.style.flexGrow = 1f;
            iconElement.style.backgroundImage = new StyleBackground(RSTheme.Current.Icon.arrow);
            iconElement.style.unityBackgroundImageTintColor = RSTheme.Current.FrontgroundColor;
            iconElement.style.scale = new Scale(new Vector3(.75f, .75f, 1f));

            iconElement2 = new VisualElement();
            iconElement2.name = "rd-icon2";
            iconElement2.style.display = DisplayStyle.None;
            iconElement2.style.height = RSTheme.Current.LineHeight;
            iconElement2.style.width = RSTheme.Current.LineHeight;
            hierarchy.Insert(1, iconElement2);
            LayoutExpand();
        }
        public override void SetIcon(Background img)
        {
            iconElement2.style.backgroundImage = img;
            if(!IsLayoutExpand)
                iconElement2.style.display = DisplayStyle.None;
            else
                iconElement2.style.display = (RSBackground.BackgroundToObject(img) != null) ? DisplayStyle.Flex : DisplayStyle.None;
        }
        public override void LayoutExpand()
        {
            base.LayoutExpand();
            m_IsLayoutExpand = true;
            var v = RSTheme.Current.BackgroundColor.GetBrightness();
            v -= ((0.5f - v) * 0.1f);
            labelElement.style.backgroundColor = RSTheme.Current.BackgroundColor.NewV(v);
            iconElement.style.opacity = 0.5f;
            FoldoutState = false;
            RegisterFoldoutStateCallback(labelElement);
        }
        public override void LayoutInline()
        {
            base.LayoutInline();
            m_IsLayoutExpand = false;
            labelElement.style.backgroundColor = Color.clear;
            iconElement.style.opacity = 0f;
            FoldoutState = true;
            UnregisterFoldoutStateCallback(labelElement);
        }

        /// <summary>
        /// this operate can't be undo
        /// </summary>
        public void DisFoldout()
        {
            var children = Children().ToList();
            hierarchy.Clear();
            hierarchy.Add(contentContainer);
            contentContainer.Clear();
            foreach (var child in children)
            {
                RuntimeDrawer drawer = child as RuntimeDrawer;
                if(drawer != null)
                {
                    float indentWidth = drawer.MeasuredIndentWidth;
                    if (drawer is IFoldoutDrawer)
                        drawer.LayoutExpand();
                    else
                        drawer.labelWidth += indentWidth;
                    foreach (var item in child.ChildrenRecursive<RuntimeDrawer>())
                    {
                        item.labelWidth += indentWidth;
                    }
                }
                contentContainer.Add(child);
            }
            style.flexDirection = FlexDirection.Column;
            contentContainer.style.flexDirection = FlexDirection.Column;
            contentContainer.style.display = DisplayStyle.Flex;
        }
        public void DisFoldoutRecursive()
        {
            Queue<IFoldoutDrawer> foldouts = new();
            foldouts.Enqueue(this);
            foreach (var drawer in this.ChildrenRecursive<RuntimeDrawer>())
            {
                IFoldoutDrawer foldout = drawer as IFoldoutDrawer;
                if (foldout != null)
                    foldouts.Enqueue(foldout);
            }
            while(foldouts.Count > 0)
                foldouts.Dequeue().DisFoldout();
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