using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public abstract class CapturePointerManipulator : PointerManipulator
    {
        protected bool IsCapture = false;
        protected virtual void PointerDown(PointerDownEvent evt)
        {
            IsCapture = true;
            target.CapturePointer(evt.pointerId);
            OnCaptured(evt);
            evt.StopPropagation();
        }
        protected virtual void PointerUp(PointerUpEvent evt)
        {
            if(target.HasPointerCapture(evt.pointerId))
            {
                IsCapture = false;
                target.ReleasePointer(evt.pointerId);
                OnReleased(evt);
                evt.StopPropagation();
            }
        }
        protected virtual void PointerMove(PointerMoveEvent evt)
        {
            if(IsCapture)
                OnDraging(evt);
        }

        protected abstract void OnCaptured(PointerDownEvent evt);
        protected abstract void OnReleased(PointerUpEvent evt);
        protected abstract void OnDraging(PointerMoveEvent evt);

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerMoveEvent>(PointerMove);
            target.RegisterCallback<PointerDownEvent>(PointerDown);
            target.RegisterCallback<PointerUpEvent>  (PointerUp);
        }
        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerMoveEvent>(PointerMove);
            target.UnregisterCallback<PointerDownEvent>(PointerDown);
            target.UnregisterCallback<PointerUpEvent>  (PointerUp);
        }
    }
}
