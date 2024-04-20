using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public abstract class NumberDrawer<T> : RuntimeDrawer<T>
    {
        RSTextField field;
        public NumberDrawer()
        {
            field = new RSTextField();
            field.RegisterCallback<FocusOutEvent>(evt =>
            {
                T result;
                if(TryParseNumber(field.value, out result))
                    SetValueWithoutRepaint(result);
                else
                    field.SetValueWithoutNotify(value.ToString());
                evt.StopPropagation();
            });
            Add(field);
            labelElement.AddManipulator(new DragManipulator(DragNumber));
        }

        protected override void CreateGUI()
        {
        }
        public override void RepaintDrawer()
        {
            if (field.IsFocusedOnPanel()) return;
            field.SetValueWithoutNotify(value.ToString());
        }
        abstract protected bool TryParseNumber(string value, out T result);
        abstract protected void DragNumber(Vector2 delta);
        class DragManipulator : PointerManipulator
        {
            public bool IsCapturing { get; private set; }
            Action<Vector2> action;
            public DragManipulator(Action<Vector2> onDraging)
            {
                action = onDraging;
            }
            private void PointerDownEvent(PointerDownEvent evt)
            {
                target.CapturePointer(evt.pointerId);
                IsCapturing = true;
                evt.StopPropagation();
            }
            private void PointerMoveEvent(PointerMoveEvent evt)
            {
                if (!IsCapturing) return;
                action(evt.deltaPosition);
            }
            private void PointerUpEvent(PointerUpEvent evt)
            {
                if (target.HasPointerCapture(evt.pointerId))
                    target.ReleasePointer(evt.pointerId);
                evt.StopPropagation();
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

    [CustomRuntimeDrawer(typeof(int), Priority = 10)]
    public class IntegerDrawer : NumberDrawer<int>
    {
        float grandTotal = 0;
        protected override void DragNumber(Vector2 delta)
        {
            var val = ((Mathf.Abs(value)+10) * (delta.x / localBound.width));
            if ((int)grandTotal != 0)
            {
                val += grandTotal;
                grandTotal = 0;
            }
            grandTotal += val % 1;
            value = value + (int)val;
        }
        protected override bool TryParseNumber(string value, out int result)
        {
            return int.TryParse(value, out result);
        }
    }
    [CustomRuntimeDrawer(typeof(float), Priority = 10)]
    public class FloatDrawer : NumberDrawer<float>
    {
        protected override void DragNumber(Vector2 delta)
        {
            var val = ((Mathf.Abs(value) + 10) * (delta.x / localBound.width));
            value = value + val;
        }

        protected override bool TryParseNumber(string value, out float result)
        {
            return float.TryParse(value, out result);
        }
    }
}