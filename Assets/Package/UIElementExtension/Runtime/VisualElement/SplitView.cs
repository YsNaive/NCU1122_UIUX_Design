using NaiveAPI.UITK;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_UI
{
    public class SplitView : VisualElement
    {
        static System.Exception _OrientationException = new System.Exception("SplitView Orientation Not Valid");
        public TwoPaneSplitViewOrientation Orientation
        {
            get => m_Orientation;
            set
            {
                var width = DragLineWidth;
                m_Orientation = value;
                DragLineWidth = width;
                viewContainer.style.flexDirection = (value == TwoPaneSplitViewOrientation.Horizontal) ? FlexDirection.Row : FlexDirection.Column;
            }
        }
        TwoPaneSplitViewOrientation m_Orientation;
        public readonly VisualElement DragLineElement;
        public override VisualElement contentContainer => viewContainer; 
        private readonly VisualElement viewContainer;
        public Length DragLineWidth
        {
            get => (m_Orientation == TwoPaneSplitViewOrientation.Horizontal) ? DragLineElement.style.width.value : DragLineElement.style.height.value;
            set
            {
                if (m_Orientation == TwoPaneSplitViewOrientation.Horizontal)
                {
                    DragLineElement.style.width = value;
                    DragLineElement.style.height = Length.Percent(100);
                }
                else
                {
                    DragLineElement.style.height = value;
                    DragLineElement.style.width = Length.Percent(100);
                }
            }
         }
        public float SplitPercent
        {
            get => m_SplitPercent;
            set
            {
                m_SplitPercent = Mathf.Clamp(value, MinViewPercent, 1f - MinViewPercent);
                solver.ExecuteLater(1);
            }
        }
        private float m_SplitPercent = 0.5f;
        public float MinViewPercent
        {
            get => m_MinViewPercent;
            set
            {
                m_MinViewPercent = value;
                SplitPercent = SplitPercent;
            }
        }
        private float m_MinViewPercent = 0.1f;
        IVisualElementScheduledItem solver;
        public SplitView(VisualElement fixedElement, VisualElement flexElement) : this()
        {
            Add(fixedElement);
            Add(flexElement);
        }
        public SplitView()
        {
            viewContainer = new();
            style.flexGrow = 1;
            viewContainer.style.flexGrow = 1;
            viewContainer.style.flexDirection = FlexDirection.Row;
            DragLineElement = new VisualElement();
            DragLineElement.style.position = Position.Absolute;
            DragLineElement.transform.position = new Vector3(100, 0);
            DragLineElement.style.backgroundColor = Color.black;
            DragLineElement.AddManipulator(new DragManipulator(this));
            hierarchy.Add(viewContainer);
            hierarchy.Add(DragLineElement);
            solver = schedule.Execute(() =>
            {
                if (childCount > 2) throw new System.Exception("You can't add more than 2 Children in SplitView.");
                var newPos = layout.size * m_SplitPercent;
                if (Orientation == TwoPaneSplitViewOrientation.Horizontal)
                    newPos.y = 0;
                else
                    newPos.x = 0;
                DragLineElement.transform.position = newPos;
                if (childCount > 0)
                {
                    var fixedElement = this[0];
                    fixedElement.style.flexGrow = m_SplitPercent;
                }
                if (childCount > 1)
                {
                    var flexElement = this[1];
                    flexElement.style.flexGrow = 1f- m_SplitPercent;
                }
            });
            RegisterCallback<GeometryChangedEvent>(evt => solver.ExecuteLater(1));
            Orientation = TwoPaneSplitViewOrientation.Horizontal;
            DragLineWidth = 3;
        }
        class DragManipulator : CapturePointerManipulator
        {
            SplitView controlTarget;
            public DragManipulator(SplitView controlTarget)
            {
                this.controlTarget = controlTarget;
            }
            protected override void OnCaptured(PointerDownEvent evt)
            {
            }

            protected override void OnDraging(PointerMoveEvent evt)
            {
                var pos = controlTarget.WorldToLocal(evt.position)/controlTarget.layout.size;
                if(controlTarget.Orientation == TwoPaneSplitViewOrientation.Horizontal)
                    controlTarget.SplitPercent = pos.x;
                else
                    controlTarget.SplitPercent = pos.y;
            }

            protected override void OnReleased(PointerUpEvent evt)
            {
            }
        }
    }
}
