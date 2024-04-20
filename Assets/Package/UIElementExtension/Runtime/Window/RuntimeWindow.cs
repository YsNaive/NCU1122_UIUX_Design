using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
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
        private static void _OpenWindow(RuntimeWindow window, string tabName)
        {
            ScreenElement.Add(window);
            if (tabName != "")
            {
                window.TabName = tabName;
                window.EnableTab = true;
            }
        }
        public static T GetWindow<T>(string tabName = "")
            where T : RuntimeWindow, new()
        {
            RuntimeWindow window;
            if (!getWindowDict.TryGetValue(typeof(T), out window))
            {
                window = new T();
                getWindowDict.Add(typeof(T), window);
            }
            _OpenWindow(window, tabName);
            return window as T;
        }
        public static T CreateWindow<T>(string tabName = "")
            where T : RuntimeWindow, new()
        {
            RuntimeWindow window = new T();
            if (!getWindowDict.ContainsKey(typeof(T)))
                getWindowDict.Add(typeof(T), window);
            _OpenWindow(window, tabName);
            return window as T;
        }
        public static void DestoryWindow(RuntimeWindow window)
        {
            window.parent?.Remove(window);
            if(getWindowDict.ContainsKey(window.GetType()))
                getWindowDict.Remove(window.GetType());
        }

        #endregion

        public RuntimeWindow()
        {
            style.position = Position.Absolute;
            layoutSolver = schedule.Execute(() =>
            {
                _solveSize();
                _solvePosition();
            });

            m_container = new VisualElement();
            m_container.style.flexGrow = 1;
            hierarchy.Add(m_container);

            TabNameElement = new TextElement();
            TabElement = new VisualElement();
            TabElement.style.flexDirection = FlexDirection.Row;
            TabElement.style.flexShrink = 0;
            TabElement.AddManipulator(new DragManipulator(this));
            TabElement.Add(TabNameElement);

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
                    PopupElement popup = RSContextMenu.CreatePopupMenu(ContextMenu.Keys.ToList(),
                        (key) => { ContextMenu[key](); });
                    popup.Open(this, evt.position);
                }
            });
            RegisterCallback<GeometryChangedEvent>(evt => { _solveLayout(); });

            _solveLayout();
        }
        public Dictionary<string, Action> ContextMenu { get; set; } = new();
        public override VisualElement contentContainer => m_container;
        private VisualElement m_container;

        public readonly VisualElement TabElement;
        protected TextElement TabNameElement;
        public string TabName
        {
            get => TabNameElement.text;
            set => TabNameElement.text = value;
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
        public bool LimitInParent
        {
            get => m_LimitInParent;
            set
            {
                m_LimitInParent = value;
                m_limitNextPosition = value;
                _solveLayout();
            }
        }
        private bool m_LimitInParent = true;
        public bool LimitSize
        {
            get => m_LimitSize;
            set
            {
                m_LimitSize = value;
                m_limitNextSize = value;
                _solveLayout();
            }
        }
        private bool m_LimitSize = true;
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
                _solveLayout();
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
                NextSize = GetCoordFromPixel(localBound.size);
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
                NextSize = GetCoordFromPixel(localBound.size);
            }
        }
        public Vector2 LastMeasureSize => m_NormalSize;
        /// <summary>
        /// Use InitLayoutAs*** to change LayoutUnit
        /// </summary>
        public LengthUnit LayoutUnit => m_LayoutUnit;
        private LengthUnit m_LayoutUnit = LengthUnit.Pixel;

        IVisualElementScheduledItem layoutSolver;
        public Vector2 NextPosition
        {
            get => m_NextPosition;
            set
            {
                m_NextPosition = value;
                _solveLayout();
            }
        }
        private Vector2 m_NextPosition;
        public Vector2 NextSize
        {
            get => m_NextSize;
            set
            {
                if (!float.IsNormal(value.x) || !float.IsNormal(value.y))
                    m_NextSize = MinSize;
                else
                    m_NextSize = value;
                _solveLayout();
            }
        }
        private Vector2 m_NextSize;
        private bool m_limitNextPosition;
        private bool m_limitNextSize;
        public void Close() { parent?.Remove(this); }
        public virtual void Destory() { DestoryWindow(this); }
        public void InitLayoutAsPercent(Rect layout) { InitLayoutAsPercent(layout, new Vector2(.1f, .1f), new Vector2(1f, 1f)); }
        public void InitLayoutAsPercent(Rect layout, Vector2 minSize, Vector2 maxSize)
        {
            m_LayoutUnit = LengthUnit.Percent;
            _InitLayout(layout, minSize, maxSize);
        }
        public void InitLayoutAsPixel(Rect layout) { InitLayoutAsPixel(layout, new Vector2(150, 100), new Vector2(3000, 2000)); }
        public void InitLayoutAsPixel(Rect layout, Vector2 minSize, Vector2 maxSize)
        {
            m_LayoutUnit = LengthUnit.Pixel;
            _InitLayout(layout, minSize, maxSize);
        }
        void _InitLayout(Rect layout, Vector2 minSize, Vector2 maxSize)
        {
            m_MinSize = minSize;
            m_MaxSize = maxSize;
            NextPosition = layout.position;
            NextSize = layout.size;
        }
        public Rect GetCoordFromPixel(Rect coord)
        {
            if (LayoutUnit == LengthUnit.Percent)
            {
                var scale = parent.worldBound.size;
                coord.position /= scale;
                coord.size /= scale;
            }
            return coord;
        }
        public Rect GetCoordFromPercent(Rect coord)
        {
            if (LayoutUnit == LengthUnit.Pixel)
            {
                var scale = parent.worldBound.size;
                coord.position *= scale;
                coord.size *= scale;
            }
            return coord;
        }
        public Vector2 GetCoordFromPixel(Vector2 coord)
        {
            if (LayoutUnit == LengthUnit.Percent)
                coord /= parent.worldBound.size;
            return coord;
        }
        public Vector2 GetCoordFromPercent(Vector2 coord)
        {
            if (LayoutUnit == LengthUnit.Pixel)
                coord *= parent.worldBound.size;
            return coord;
        }
        public void SetPositionRelative(Vector2 offset)
        {
            m_NextPosition += offset;
            _solveLayout();
        }
        public void SetPosition(Vector2 position)
        {
            NextPosition = position;
        }
        public void SetPositionAnyway(Vector2 position)
        {
            m_limitNextPosition = false;
            NextPosition = position;
        }
        public void SetSize(Vector2 size)
        {
            NextSize = size;
        }
        public void SetSizeAnyway(Vector2 size)
        {
            m_limitNextSize = false;
            this.NextSize = size;
        }
        public void SetLayout(Rect layout)
        {
            NextSize = layout.size;
            NextPosition = layout.position;
        }
        public void SetLayoutAnyway(Rect layout)
        {
            SetSizeAnyway(layout.size);
            SetPositionAnyway(NextPosition = layout.position);
        }
        private void _solvePosition()
        {
            if (parent == null) return;
            var bounding = parent.worldBound;
            var layout = this.layout;
            if (LayoutUnit == LengthUnit.Pixel)
                layout.position = m_NextPosition;
            else
                layout.position = m_NextPosition * bounding.size;
            layout.position = parent.LocalToWorld(layout.position);
            if (SnapBorder != TextAnchor.MiddleCenter)
            {
                int snap = (int)SnapBorder;
                var pos = layout.position;
                if (snap % 3 == 0)
                    pos.x = 0;
                else if (snap % 3 == 2)
                    pos.x = bounding.width - layout.width;
                if (snap / 3 == 0)
                    pos.y = 0;
                else if (snap / 3 == 2)
                    pos.y = bounding.height - layout.height;
                layout.position = pos;
            }

            if (m_limitNextPosition)
            {
                var pos = layout.position;
                if (pos.x < bounding.x)
                    pos.x = bounding.x;
                if (pos.x + layout.width > bounding.xMax)
                    pos.x = bounding.xMax - layout.width;
                if (pos.y < bounding.yMin)
                    pos.y = bounding.y;
                if (pos.y + layout.height > bounding.yMax)
                    pos.y = bounding.yMax - layout.height;
                layout.position = pos;
            }
            m_limitNextPosition = m_LimitInParent;
            layout.position = parent.WorldToLocal(layout.position);
            transform.position = layout.position;
        }
        private void _solveSize()
        {
            var bounding = parent.worldBound;
            var size = m_NextSize;
            if (m_limitNextSize)
            {
                size.x = Mathf.Clamp(size.x, MinSize.x, MaxSize.x);
                size.y = Mathf.Clamp(size.y, MinSize.y, MaxSize.y);
                m_NormalSize = size;
            }
            m_limitNextSize = LimitSize;
            if (LayoutUnit == LengthUnit.Percent)
                size *= bounding.size;

            style.width = size.x;
            style.height = size.y;
            _solveLayout();
        }
        private void _solveLayout()
        {
            layoutSolver.ExecuteLater(1);
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

        public class DragManipulator : PointerManipulator
        {
            public bool IsCapturing { get; private set; }
            public RuntimeWindow TargetWindow { get; private set; }
            private Vector3 coordOffset;
            private TextAnchor snapAnchor;
            public DragManipulator(VisualElement targetWindow)
            {
                if (!typeof(RuntimeWindow).IsAssignableFrom(targetWindow.GetType()))
                    throw new Exception("You can only add this Manipulator on a RuntimeWindow element.");
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
                TargetWindow.NextPosition = TargetWindow.GetCoordFromPixel(evt.position - coordOffset);
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
                        TargetWindow.NextSize = TargetWindow.LastMeasureSize;
                    else
                    {
                        TargetWindow.SetLayoutAnyway(TargetWindow.GetCoordFromPercent(LayoutPercent.FromAnchor(nextAnchor)));
                    }
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
        public class ResizeManipulator : PointerManipulator
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
            public ResizeManipulator(Func<Vector2, TextAnchor> getResizeAnchor, Action<TextAnchor> onHoverResizeArea, Action<TextAnchor> onLeaveResizeArea)
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
                    if (x != 1) size.x += delta.x;
                    if (y != 1) size.y += delta.y;
                    target.NextSize = target.GetCoordFromPixel(size);
                    var localBound = target.layout;
                    if (x == 0) pos.x = originWorldBound.xMax - localBound.width;
                    if (y == 0) pos.y = originWorldBound.yMax - localBound.height;
                    target.NextPosition = target.GetCoordFromPixel(pos);
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
            public static Rect FullScreen => new Rect(1f, 1f, 1f, 1f);
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