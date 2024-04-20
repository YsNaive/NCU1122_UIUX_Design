using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(IList), DrawAssignableType = true, Priority = 0)]
    public class ArrayDrawer : FoldoutDrawer<IList>
    {
        public override bool DynamicLayout => true;
        IntegerDrawer countDrawer;
        Type itemType;
        bool isDraging;
        public ArrayDrawer()
        {
            countDrawer = new IntegerDrawer();
            countDrawer.style.marginRight = StyleKeyword.Auto;
            countDrawer.style.minWidth = RSTheme.Current.LineHeight * 5;
            countDrawer.RegisterCallback<PointerDownEvent>(evt => { evt.StopPropagation(); });
            countDrawer.OnValueChanged += () =>
            {
                while (value.Count > countDrawer.value)
                    value.RemoveAt(value.Count - 1);
                while (value.Count < countDrawer.value)
                    value.Add(Activator.CreateInstance(itemType));
            };
            labelElement.Add(countDrawer);
        }
        public override void RepaintDrawer()
        {
            if (isDraging) return;
            if (childCount != (value?.Count ?? 0))
                repaintList();
            for(int i=0,imax = childCount; i < imax; i++)
            {
                if ((this[i] as IFoldoutDrawer)?.FoldoutState ?? true)
                    ((RuntimeDrawer)this[i]).SetValueWithoutNotify(value[i]);
            }
        }

        protected override void CreateGUI()
        {
            itemType = null;
            if (value.GetType().IsGenericType)
            {
                if (typeof(List<>) == value.GetType().GetGenericTypeDefinition())
                    itemType = value.GetType().GetGenericArguments()[0];
            }
            else if (value.GetType().IsArray)
            {
                itemType = value.GetType().GetElementType();
            }
            repaintList();
        }

        void repaintList()
        {
            Clear();
            countDrawer.SetValueWithoutNotify(value.Count);
            countDrawer.SetEnabled(!value.IsFixedSize);
            for(int i=0,imax = value.Count;i<imax;i++)
            {
                var drawer = Create(value[i], $"= Item {i}");
                var w = RSTheme.Current.LineHeight * 4;
                if(drawer is IFoldoutDrawer)
                    drawer.labelElement.Add(new VisualElement { style = { flexGrow = 1 } });
                drawer.labelElement.style.flexGrow = 0;
                drawer.labelElement.style.minWidth = w;
                drawer.labelElement.style.width    = w;
                drawer.labelElement.AddManipulator(new ItemDragManipulator(this, drawer));
                Add(drawer);
                drawer.OnValueChanged += () => value[drawer.parent.IndexOf(drawer)] = drawer.GetValue();
                
            }
        }

        class ItemDragManipulator : PointerManipulator
        {
            public bool IsCapturing { get; private set; }
            public ArrayDrawer arrayDrawer;
            public RuntimeDrawer itemDrawer;
            int fromIndex, toIndex;
            public ItemDragManipulator(ArrayDrawer arrayDrawer, RuntimeDrawer itemDrawer)
            {
                this.arrayDrawer = arrayDrawer;
                this.itemDrawer = itemDrawer;
            }

            private void PointerDownEvent(PointerDownEvent evt)
            {
                target.CapturePointer(evt.pointerId);
                IsCapturing = true;
                evt.StopPropagation();
                arrayDrawer.isDraging = true;
                fromIndex = arrayDrawer.IndexOf(itemDrawer);
                foreach(var ve in arrayDrawer.Children())
                    ve.SetEnabled(ve == itemDrawer);
            }

            private void PointerMoveEvent(PointerMoveEvent evt)
            {
                if (!IsCapturing) return;
                toIndex = 0;
                for(int imax = arrayDrawer.childCount; toIndex < imax; toIndex++)
                {
                    if (arrayDrawer[toIndex].worldBound.yMax > evt.position.y)
                        break;
                }
                if (toIndex >= arrayDrawer.childCount)
                    toIndex = arrayDrawer.childCount-1;
                arrayDrawer.Insert(toIndex, itemDrawer);
            }

            private void PointerUpEvent(PointerUpEvent evt)
            {
                if (target.HasPointerCapture(evt.pointerId))
                {
                    target.ReleasePointer(evt.pointerId);
                    evt.StopPropagation();

                    if (toIndex == fromIndex) return;
                    arrayDrawer.value.Insert(toIndex < fromIndex ? toIndex: toIndex + 1, arrayDrawer.value[fromIndex]);
                    arrayDrawer.value.RemoveAt(fromIndex < toIndex ? fromIndex : fromIndex + 1);
                    arrayDrawer.RepaintDrawer();
                }
            }

            private void PointerCaptureOutEvent(PointerCaptureOutEvent evt)
            {
                IsCapturing = false;
                arrayDrawer.isDraging = false;
                foreach (var ve in arrayDrawer.Children())
                    ve.SetEnabled(true);
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