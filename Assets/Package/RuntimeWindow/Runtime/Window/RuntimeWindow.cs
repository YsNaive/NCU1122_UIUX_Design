using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    public abstract class RuntimeWindow : VisualElement
    {
        #region static part
        // old value, new value
        public static event Action<VisualElement, VisualElement> OnScreenElementChanged;
        public static VisualElement ScreenElement
        {
            get
            {
                if (m_ScreenElement == null)
                    throw new Exception("RuntimeWindow ScreenElement not found.");
                else return m_ScreenElement;
            }
            set {
                if (m_ScreenElement == value) return;
                var oldVal = m_ScreenElement;
                m_ScreenElement = value;
                OnScreenElementChanged?.Invoke(oldVal, value);
            }
        }
        private static VisualElement m_ScreenElement;

        private static Dictionary<Type, RuntimeWindow> getWindowDict = new();
        public static T GetWindow<T>(string tabName = "")
            where T : RuntimeWindow, new()
        {
            RuntimeWindow window;
            if (!getWindowDict.TryGetValue(typeof(T), out window))
            {
                window = new T();
                getWindowDict.Add(typeof(T), window);
            }
            ScreenElement.Add(window);
            if(tabName != "")
                window.TabName = tabName;
            window.EnableTab = true;

            return window as T;
        }
        public static T CreateWindow<T>(string tabName = "")
            where T : RuntimeWindow, new()
        {
            RuntimeWindow window = new T();
            if (!getWindowDict.ContainsKey(typeof(T)))
                getWindowDict.Add(typeof(T), window);
            ScreenElement.Add(window);
            return window as T;
        }
        public static void DestoryWindow(RuntimeWindow window)
        {
            if (window.parent != null)
                window.parent.Remove(window);
            if(getWindowDict.ContainsKey(window.GetType()))
                getWindowDict.Remove(window.GetType());
        }

        #endregion

        public RuntimeWindow() {
            style.position = Position.Absolute;
            SetSize(MinSize);

            m_container = new VisualElement();
            m_container.style.flexGrow = 1;
            hierarchy.Add(m_container);

            tabNameElement = new TextElement();
            TabElement = new VisualElement();
            TabElement.style.flexDirection = FlexDirection.Row;
            TabElement.style.flexShrink = 0;
            TabElement.AddManipulator(new PointerDragManipulator(this));
            TabElement.Add(tabNameElement);

            RegisterCallback<PointerDownEvent>(evt =>
            {
                if (PopupOnClick)
                {
                    if (tryBringToFront())
                    {
                        evt.StopPropagation();
                        return;
                    }
                }
                if (EnableContextMenu && evt.button == (int)MouseButton.RightMouse)
                {
                    if (ContextMenu.Count == 0) return;
                    PopupElement popup = DSStringMenu.CreatePopupMenu(ContextMenu.Keys.ToList(),
                        (key) => { ContextMenu[key](); });
                    popup.Open(this);
                    popup.transform.position = popup.CoverMask.WorldToLocal(evt.position);
                }
            });
            RegisterCallback<GeometryChangedEvent>(evt => { solveSnapBorder(); });
        }
        public Dictionary<string, Action> ContextMenu { get; set; } = new();
        public override VisualElement contentContainer => m_container;
        private VisualElement m_container;

        public readonly VisualElement TabElement;
        protected TextElement tabNameElement;
        public string TabName
        {
            get => tabNameElement.text;
            set => tabNameElement.text = value;
        }
        public bool EnableTab
        {
            get => TabElement.parent == this;
            set
            {
                if (value == EnableTab) return;
                if (value)
                    hierarchy.Insert(0, TabElement);
                else
                    hierarchy.Remove(TabElement);
            }
        }
        public bool EnableContextMenu { get; set; } = true;
        public bool LimitInParent { get; set; } = true;
        public bool LimitSize { get; set; } = true;
        public bool Dragable { get; set; } = true;
        public bool Resizable { get; set; } = true;
        public bool SnapLayout { get; set; } = false;
        // once you set this property, SnapLayout will set to false
        public TextAnchor SnapBorder
        {
            get => m_SnapBorder;
            set
            {
                SnapLayout = false;
                m_SnapBorder = value;
                solveSnapBorder();
            }
        }
        private TextAnchor m_SnapBorder = TextAnchor.MiddleCenter;
        public bool PopupOnClick { get; set; } = true;

        private Vector2 m_MinSize = new Vector2(150, 100);
        private Vector2 m_MaxSize = new Vector2(450, 300);
        private Vector2 m_NormalSize;
        public Vector2 MinSize
        {
            get => m_MinSize;
            set
            {
                m_MinSize = value;
                if (m_MaxSize.x < value.x) m_MaxSize = new Vector2(value.x+1, m_MaxSize.y);
                if (m_MaxSize.y < value.y) m_MaxSize = new Vector2(m_MaxSize.x, value.y+1);
                SetSize(localBound.size);
            }
        }
        public Vector2 MaxSize
        {
            get => m_MaxSize;
            set
            {
                m_MaxSize = value;
                if (m_MinSize.x > value.x) m_MinSize = new Vector2(value.x - 1, m_MinSize.y);
                if (m_MinSize.y > value.y) m_MinSize = new Vector2(m_MinSize.x, value.y + 1);
                SetSize(localBound.size);
            }
        }
        public Vector2 NormalSize => m_NormalSize;

        public void Close() { parent?.Remove(this); }
        public virtual void Destory() { DestoryWindow(this); }
        public Vector2 SetPositionRelative(Vector2 offset) { return SetPosition((Vector2)transform.position + offset); }
        public Vector2 SetPositionPercent(Vector2 percent) { return SetPosition(percent * parent.layout.size); }
        public Vector2 SetPositionAnyway(Vector2 positoin)
        {
            transform.position = positoin;
            return transform.position;
        }
        public Vector2 SetPosition(Vector2 position)
        {
            if (LimitInParent)
            {
                var bound = parent.worldBound;
                var selfBound = worldBound;
                if (position.x < bound.x)
                    position.x = bound.x;
                if (position.x + selfBound.width > bound.xMax) 
                    position.x = bound.xMax - selfBound.width;
                if (position.y < bound.yMin)
                    position.y = bound.y;
                if (position.y + selfBound.height > bound.yMax)
                    position.y = bound.yMax - selfBound.height;
                transform.position = position;
            }
            else
            {
                transform.position = position;
            }
            solveSnapBorder();
            return transform.position;
        }
        public Vector2 SetSizeAnyway(Vector2 size)
        {
            style.width = size.x;
            style.height = size.y;
            return size;
        }
        public Vector2 SetSize(Vector2 size)
        {
            if (!float.IsNormal(size.x) || !float.IsNormal(size.y))
                size = MinSize;
            else if (LimitSize) 
            {
                size.x = Mathf.Clamp(size.x, MinSize.x, MaxSize.x);
                size.y = Mathf.Clamp(size.y, MinSize.y, MaxSize.y);
            }
            style.width  = size.x;
            style.height = size.y;
            m_NormalSize = size;
            MarkDirtyRepaint();
            solveSnapBorder();
            return size;
        }
        public Rect SetLayoutPercent(Rect layoutPercent) 
        { return SetLayout(new Rect(layoutPercent.position * parent.localBound.size, layoutPercent.size * parent.localBound.size)); }
        public Rect SetLayout(Rect worldBound)
        {
            worldBound.size = SetSize(worldBound.size);
            worldBound.position = SetPosition(worldBound.position);
            solveSnapBorder();
            return worldBound;
        }
        public Rect SetLayoutPercentAnyway(Rect layoutPercent)
        { return SetLayoutAnyway(new Rect(layoutPercent.position * parent.localBound.size, layoutPercent.size * parent.localBound.size)); }
        public Rect SetLayoutAnyway(Rect worldBound)
        {
            SetPositionAnyway(worldBound.position);
            SetSizeAnyway(worldBound.size);
            return worldBound;
        }

        private void solveSnapBorder()
        {
            if (SnapBorder == TextAnchor.MiddleCenter)
                return;
            int snap = (int)SnapBorder;
            var pos = transform.position;
            if (snap % 3 == 0)
                pos.x = 0;
            else if (snap % 3 == 2)
                pos.x = parent?.worldBound.width - localBound.width ?? 0;
            if (snap / 3 == 0)
                pos.y = 0;
            else if (snap / 3 == 2)
                pos.y = parent?.worldBound.height - localBound.height ?? 0;
            transform.position = pos;
        }
        private bool tryBringToFront()
        {
            if (parent == null) return false;
            int i = parent.childCount - 1;
            for(; i >= 0; i--)
            {
                if (parent[i] is RuntimeWindow)
                    break;
            }
            if (i == parent.IndexOf(this))
                return false;
            parent.Insert(i, this);
            return true;
        }

        public class PointerDragManipulator : PointerManipulator
        {
            public bool IsCapturing { get; private set; }
            public RuntimeWindow TargetWindow { get; private set; }
            private Vector3 coordOffset;
            private TextAnchor snapAnchor;
            public PointerDragManipulator(VisualElement targetWindow)
            {
                if (!typeof(RuntimeWindow).IsAssignableFrom(targetWindow.GetType()))
                    throw new Exception("You can't add this Manipulator by target on a NonRuntimeWindow element.");
                TargetWindow = targetWindow as RuntimeWindow;
            }

            private void PointerDownEvent(PointerDownEvent evt)
            {
                evt.StopPropagation();
                if (TargetWindow.PopupOnClick)
                {
                    if (TargetWindow.tryBringToFront())
                        return;
                }
                if (!TargetWindow.Dragable) return;
                target.CapturePointer(evt.pointerId);
                IsCapturing = true;
                coordOffset = target.parent.WorldToLocal(evt.position); 
            }

            private void PointerMoveEvent(PointerMoveEvent evt)
            {
                if (!IsCapturing) return;
                TargetWindow.SetPosition(evt.position - coordOffset);
                if (!TargetWindow.SnapLayout) return;
                var pos = TargetWindow.parent.WorldToLocal(evt.position);
                pos /= TargetWindow.parent.localBound.size;
                int anchor = 0;
                if (pos.x < .05f)
                    anchor += 0;
                else if (pos.x > .95f)
                    anchor += 2;
                else
                    anchor += 1;

                if (pos.y < .05f)
                    anchor += 0;
                else if (pos.y > .95f)
                    anchor += 6;
                else
                    anchor += 3;
                var nextAnchor = (TextAnchor)anchor;
                if (nextAnchor != TextAnchor.MiddleCenter || nextAnchor != snapAnchor)
                {
                    if (nextAnchor == TextAnchor.MiddleCenter)
                        TargetWindow.SetSize(TargetWindow.NormalSize);
                    else
                        TargetWindow.SetLayoutPercentAnyway(LayoutPercent.FromAnchor(nextAnchor));
                    snapAnchor = nextAnchor;
                }
            }

            private void PointerUpEvent(PointerUpEvent evt)
            {
                if (target.HasPointerCapture(evt.pointerId))
                {
                    target.ReleasePointer(evt.pointerId);
                    evt.StopPropagation();
                }
            }

            private void PointerCaptureOutEvent(PointerCaptureOutEvent evt)
            {
                IsCapturing = false;
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<PointerDownEvent>(PointerDownEvent);
                target.RegisterCallback<PointerMoveEvent>(PointerMoveEvent);
                target.RegisterCallback<PointerUpEvent>(PointerUpEvent);
                target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutEvent);
            }
             
            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<PointerDownEvent>(PointerDownEvent);
                target.UnregisterCallback<PointerMoveEvent>(PointerMoveEvent);
                target.UnregisterCallback<PointerUpEvent>(PointerUpEvent);
                target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutEvent);
            }

        }
        public class PointerResizeManipulator : PointerManipulator
        {
            private Func<Vector2, TextAnchor> getResizeAnchor;
            Action<TextAnchor> onHoverResizeArea, onLeaveResizeArea;
            public bool IsCapturing { get; private set; }
            private new RuntimeWindow target
            {
                get => base.target as RuntimeWindow;
                set {
                    if (!typeof(RuntimeWindow).IsAssignableFrom(base.target.GetType()))
                        throw new Exception("You can't add this Manipulator on a NonRuntimeWindow element.");
                    base.target = value;
                }
            }
            TextAnchor ResizingType = TextAnchor.MiddleCenter;
            private Vector2 beginPosition;
            private Rect originWorldBound;
            public PointerResizeManipulator(Func<Vector2, TextAnchor> getResizeAnchor, Action<TextAnchor> onHoverResizeArea, Action<TextAnchor> onLeaveResizeArea)
            {
                this.getResizeAnchor = getResizeAnchor;
                this.onHoverResizeArea = onHoverResizeArea;
                this.onLeaveResizeArea = onLeaveResizeArea;
                IsCapturing = false;
            }

            private void PointerDownEvent(PointerDownEvent evt) 
            {
                if (!target.Resizable) return;
                if (ResizingType == TextAnchor.MiddleCenter) return;
                beginPosition = evt.position;
                originWorldBound = target.worldBound;
                target.CapturePointer(evt.pointerId);
                IsCapturing = true;
                evt.StopPropagation();
            }
            private void PointerMoveEvent(PointerMoveEvent evt)
            {
                if (!target.Resizable) return;
                if (IsCapturing)
                {
                    int x = (int)ResizingType % 3;
                    int y = (int)ResizingType / 3;
                    Vector2 delta = beginPosition - (Vector2)evt.position;
                    Vector2 pos = originWorldBound.position;
                    Vector2 size = originWorldBound.size;
                    if (x == 2) delta.x *= -1;
                    if (y == 2) delta.y *= -1;
                    if (x != 1) size.x = originWorldBound.width  + delta.x;
                    if (y != 1) size.y = originWorldBound.height + delta.y;
                    size = size - target.SetSize(size);
                    if (x == 0) pos.x -= delta.x - size.x;
                    if (y == 0) pos.y -= delta.y - size.y;
                    target.SetPosition(pos);

                    return;
                }
                var type = getResizeAnchor(target.WorldToLocal(evt.position));
                if (type == ResizingType) return; ResizingType = type;
                if (ResizingType == TextAnchor.MiddleCenter)
                {
                    onLeaveResizeArea?.Invoke(ResizingType);
                    return;
                }
                onHoverResizeArea?.Invoke(ResizingType);
            }
            private void PointerUpEvent(PointerUpEvent evt)
            {
                if (target.HasPointerCapture(evt.pointerId))
                {
                    target.ReleasePointer(evt.pointerId);
                    evt.StopPropagation();
                }
            }
            private void PointerCaptureOutEvent(PointerCaptureOutEvent evt)
            {
                onLeaveResizeArea?.Invoke(ResizingType);
                IsCapturing = false;
                evt.StopPropagation();
            }
            private void PointerLeaveEvent(PointerLeaveEvent evt)
            {
                if (IsCapturing) return;
                if(ResizingType != TextAnchor.MiddleCenter)
                {
                    onLeaveResizeArea?.Invoke(ResizingType);
                    ResizingType = TextAnchor.MiddleCenter;
                }
            }
            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<PointerDownEvent>(PointerDownEvent);
                target.RegisterCallback<PointerMoveEvent>(PointerMoveEvent);
                target.RegisterCallback<PointerUpEvent>(PointerUpEvent);
                target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutEvent);
                target.RegisterCallback<PointerLeaveEvent>(PointerLeaveEvent);
            }
            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<PointerDownEvent>(PointerDownEvent);
                target.UnregisterCallback<PointerMoveEvent>(PointerMoveEvent);
                target.UnregisterCallback<PointerUpEvent>(PointerUpEvent);
                target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutEvent);
                target.UnregisterCallback<PointerLeaveEvent>(PointerLeaveEvent);
            }
        }
        public static class LayoutPercent
        {
            public static Rect FullScreen = new Rect(1f, 1f, 1f, 1f);
            public static Rect UpperHalf => new Rect(0f, 0f, 1f, .5f);
            public static Rect BottomHalf => new Rect(0f, .5f, 1f, .5f);
            public static Rect LeftHalf => new Rect(0f, 0f, .5f, 1f);
            public static Rect LeftOneThird => new Rect(0f, 0f, .33f, 1f);
            public static Rect RightHalf => new Rect(.5f, 0f, .5f, 1f);
            public static Rect RightOneThird => new Rect(.67f, 0f, .33f, 1f);
            public static Rect UpperLeft => new Rect(0f, 0f, .5f, .5f);
            public static Rect UpperRight => new Rect(.5f, 0f, .5f, .5f);
            public static Rect BottomLeft => new Rect(0f, .5f, .5f, .5f);
            public static Rect BottomRight => new Rect(.5f, .5f, .5f, .5f);
            public static Rect FromAnchor(TextAnchor anchor)
            {
                return anchor switch
                {
                    TextAnchor.UpperLeft => UpperLeft,
                    TextAnchor.UpperCenter => UpperHalf,
                    TextAnchor.UpperRight => UpperRight,
                    TextAnchor.MiddleLeft => LeftHalf,
                    TextAnchor.MiddleCenter => FullScreen,
                    TextAnchor.MiddleRight => RightHalf,
                    TextAnchor.LowerLeft => BottomLeft,
                    TextAnchor.LowerCenter => BottomHalf,
                    TextAnchor.LowerRight => BottomRight,
                    _ => FullScreen
                };
            }
        }
    }
}