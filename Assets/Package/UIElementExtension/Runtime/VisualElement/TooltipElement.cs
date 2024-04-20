using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class TooltipElement : PopupElement
    {
        Vector2 defaultPositionProcess(Rect worldBound)
        {
            var position = hoveringPosition;
            if (position.y > parent.worldBound.center.y)
                position.y -= worldBound.height + RSTheme.Current.LineHeight / 2f;
            else
                position.y += RSTheme.Current.LineHeight    ;
            position.x -= worldBound.width / 2;
            return position;
        }
        public Func<Rect, Vector2> ProcessOpenPosition;
        /// <summary>
        /// unit = mili sec
        /// </summary>
        public int PopupDelay = 1000;
        bool isHovering => hoveringTarget != null;
        IEventHandler hoveringTarget = null;
        Vector2 hoveringPosition = Vector2.zero;
        HashSet<VisualElement> popupOnElements = new();

        IVisualElementScheduledItem updatePosition;
        IVisualElementScheduledItem updateShowup;
        public TooltipElement(VisualElement scheduleTarget)
        {
            pickingMode = PickingMode.Ignore;
            CoverMask.pickingMode = PickingMode.Ignore;

            ProcessOpenPosition = defaultPositionProcess;
            updatePosition = scheduleTarget.schedule.Execute(() =>
            {
                var pos = ProcessOpenPosition(new Rect(hoveringPosition, worldBound.size));
                pos = parent.WorldToLocal(pos);
                style.top = pos.y;
                style.left = pos.x;
                style.opacity = 1;
                updatePosition.Pause();
            });
            updatePosition.Pause();
            updateShowup = scheduleTarget.schedule.Execute(() =>
            {
                if(isHovering && !IsOpend)
                {
                    style.opacity = 0;
                    Open(hoveringTarget as VisualElement, Vector2.zero);
                    updatePosition.ExecuteLater(1);
                }
                updateShowup.Pause();
            });
            updateShowup.Pause();
            RegisterCallback<PointerLeaveEvent>(evt => { if (hoveringTarget == null) tryClose(evt.position); });
            RegisterCallback<PointerMoveEvent>(evt => { if (hoveringTarget == null) tryClose(evt.position); });
        }
        void PointerMove(PointerMoveEvent evt)
        {
            hoveringPosition = evt.position;
        }
        void PointerEnter(PointerEnterEvent evt)
        {
            if (hoveringTarget == null)
                updateShowup.ExecuteLater(PopupDelay);
            hoveringTarget = evt.target;
        }
        void PointerLeave(PointerLeaveEvent evt)
        {
            if (hoveringTarget == evt.target)
            {   
                hoveringTarget = null;
                tryClose(evt.position);
            }
        }
        void tryClose(Vector2 worldPosition)
        {
            foreach(var ve in popupOnElements)
            {
                if (ve.worldBound.Contains(worldPosition))
                    return;
            }
            Close();
        }
        public void RegisterPopupOnTarget(VisualElement target)
        {
            target.RegisterCallback<PointerMoveEvent>(PointerMove);
            target.RegisterCallback<PointerEnterEvent>(PointerEnter);
            target.RegisterCallback<PointerLeaveEvent>(PointerLeave);
            popupOnElements.Add(target);
        }
        public void UnregisterPopupOnTarget(VisualElement target)
        {
            target.UnregisterCallback<PointerMoveEvent>(PointerMove);
            target.UnregisterCallback<PointerEnterEvent>(PointerEnter);
            target.UnregisterCallback<PointerLeaveEvent>(PointerLeave);
            popupOnElements.Remove(target);
        }
    }
}
