using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSSlider : VisualElement, INotifyValueChanged<float>
    {
        public float lowValue;
        public float highValue;
        public float value
        {
            get => m_value;
            set
            {
                value = Mathf.Clamp(value, lowValue, highValue);
                if (m_value == value) return;
                using var evt = ChangeEvent<float>.GetPooled(m_value, value);
                evt.target = this;
                SendEvent(evt);
                m_value = value;
                dragElement.transform.position = new Vector3((value-lowValue)/(highValue-lowValue) * (maxDragPosisiton), 0f, 0f);
            }
        }
        float m_value = 0f;
        public VisualElement dragElement;
        public VisualElement dragLineElement;
        private float maxDragPosisiton;
        DragManipulator dragElementManipulator;
        public RSSlider()
        {
            style.minHeight = RSTheme.Current.LineHeight;
            dragElement = new VisualElement();
            dragElement.name = "drag-element";
            dragElement.style.position = Position.Absolute;
            dragElement.style.width = RSTheme.Current.LineHeight / 5f;
            dragElement.style.height = Length.Percent(70);
            dragElement.style.top = Length.Percent(15);
            dragElement.style.backgroundColor = RSTheme.Current.FrontgroundColor;
            dragElementManipulator = new DragManipulator(this);
            dragElement.AddManipulator(dragElementManipulator);
            dragLineElement = new VisualElement();
            dragLineElement.name = "drag-line-element";
            dragLineElement.style.width = Length.Percent(100);
            dragLineElement.style.height = Length.Percent(20);
            dragLineElement.style.top = Length.Percent(40);
            dragLineElement.style.backgroundColor = RSTheme.Current.BackgroundColor2;
            dragLineElement.RegisterCallback<PointerDownEvent>(_beginDrag);
            Add(dragLineElement);
            Add(dragElement);

            RegisterCallback<GeometryChangedEvent>(_calculateDragBound);
            dragElement.RegisterCallback<GeometryChangedEvent>(_calculateDragBound);
        }
        void _calculateDragBound(GeometryChangedEvent evt)
        {
            maxDragPosisiton = layout.width - dragElement.layout.width;
        }
        void _beginDrag(PointerDownEvent evt)
        {
            dragElementManipulator.PointerDownEvent(evt);
        }
        public void SetValueWithoutNotify(float newValue)
        {
            m_value = newValue;
        }

        class DragManipulator : PointerManipulator
        {
            public bool IsCapturing { get; private set; }
            RSSlider slider;
            public DragManipulator(RSSlider slider)
            {
                this.slider = slider;
            }

            public void PointerDownEvent(PointerDownEvent evt)
            {
                evt.StopPropagation();
                target.CapturePointer(evt.pointerId);
                IsCapturing = true;
            }
            private void PointerMoveEvent(PointerMoveEvent evt)
            {
                if (!IsCapturing) return;
                var pos = evt.position;
                pos = slider.WorldToLocal(pos);
                pos.x = Mathf.Clamp(pos.x, 0, slider.maxDragPosisiton);
                pos.y = 0;
                slider.value = ((slider.highValue - slider.lowValue) * (pos.x / slider.maxDragPosisiton)) + slider.lowValue;
                target.transform.position = pos;
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

    }
}
