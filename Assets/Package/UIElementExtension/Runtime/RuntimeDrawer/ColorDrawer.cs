using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(Color))]
    public class ColorDrawer : RuntimeDrawer<Color>
    {
        const string CachePath = "colorDrawerCache.json";
        static ColorPicker colorPicker;
        static Dictionary<int, Texture2D> containerBackgroundTable = new();
        static Action<Color> onColorChanged;
        static ColorDrawer()
        {
            colorPicker = new();
            colorPicker.OnColorChanged += (c) => { onColorChanged?.Invoke(c); };
            colorPicker.OnClosed += () => { 
                onColorChanged = null;
                NaiveAPICache.Save(CachePath, JsonUtility.ToJson(new Vector2(colorPicker.style.left.value.value, colorPicker.style.top.value.value)));
            };
            colorPicker.OnOpend += (ve) => {
                string data = NaiveAPICache.Load(CachePath);
                if (data != null)
                {
                    Vector2 pos = JsonUtility.FromJson<Vector2>(data);
                    colorPicker.style.left = pos.x;
                    colorPicker.style.top = pos.y;
                }
                onColorChanged = null; 
            };
        }
        internal static Texture2D getContainerBackground(Vector2 size)
        {
            if (!float.IsNormal(size.x)) size.x = 1;
            if (!float.IsNormal(size.y)) size.y = 1;
            if (size.y == 0) size.y = 1;
            var key = (int)((size.x*3) / size.y);
            if (key <= 0) key = 1;
            if(containerBackgroundTable.TryGetValue(key, out var texture))
                return texture;
            texture = new Texture2D(key, 3);
            for(int i=0;i<key; i++)
            {
                var c1 = i % 2 == 0 ? Color.white : Color.gray;
                var c2 = i % 2 == 0 ? Color.gray : Color.white;
                texture.SetPixel(i, 0, c1);
                texture.SetPixel(i, 1, c2);
                texture.SetPixel(i, 2, c1);
            }
            texture.filterMode = FilterMode.Point;
            texture.Apply();
            containerBackgroundTable.Add(key, texture);
            return texture;
        }

        public bool Dragable { get; set; } = true;
        VisualElement colorElement;
        VisualElement colorBGElement;
        VisualElement eyedropperButton;
        protected override void CreateGUI()
        {
            contentContainer.style.flexDirection = FlexDirection.Row;
            colorBGElement = new VisualElement
            {
                style =
                {
                    height = Length.Percent(100),
                    width = MeasuredLabelWidth,
                    maxWidth = MeasuredLabelWidth,
                }
            };
            colorBGElement.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                if (evt.newRect.height < 1 || evt.newRect.width < 1) return;
                colorBGElement.style.backgroundImage = Background.FromTexture2D(getContainerBackground(evt.newRect.size));
            });
            colorElement = new VisualElement();
            RSBorder.op_temp.anyWidth = RSTheme.Current.VisualMargin / 2f;
            RSBorder.op_temp.anyColor = RSTheme.Current.BackgroundColor2;
            colorElement.style.SetRS_Style(RSSize.Full);
            colorElement.style.SetRS_Style(RSBorder.op_temp);
            colorElement.RegisterCallback<PointerDownEvent>(evt =>
            {
                evt.StopImmediatePropagation();
                colorPicker.Dragable = Dragable;
                colorPicker.Open(this, evt.position);
                colorPicker.defaultColor = value;
                onColorChanged += (newColor) =>
                {
                    value = newColor;
                };
            });
            colorBGElement.Add(colorElement);
            Add(colorBGElement);
        }
        public override void RepaintDrawer()
        {
            colorElement.style.backgroundColor = value;
        }

        //class EyedropperManipulator : PointerManipulator
        //{
        //    bool isPicking = false;
        //    void PointerDown(PointerDownEvent evt)
        //    {
        //        if (!isPicking)
        //        {
        //            evt.StopImmediatePropagation();
        //            target.CapturePointer(evt.pointerId);
        //            isPicking = true;
        //        }
        //        else
        //        {
        //            isPicking = false;
        //        }
        //    }
        //    void PointerMove(PointerMoveEvent evt)
        //    {
        //        if(isPicking)
        //        {
        //            evt.StopImmediatePropagation();
        //            var screenPos = (evt.position / target.panel.visualTree.worldBound.size) * new Vector2(Screen.width, Screen.height);
        //            Debug.Log(screenPos);
        //        }
        //    }
        //    protected override void RegisterCallbacksOnTarget()
        //    {
        //        target.RegisterCallback<PointerDownEvent>(PointerDown);
        //        target.RegisterCallback<PointerMoveEvent>(PointerMove);
        //    }

        //    protected override void UnregisterCallbacksFromTarget()
        //    {
        //        target.UnregisterCallback<PointerDownEvent>(PointerDown);
        //        target.UnregisterCallback<PointerMoveEvent>(PointerMove);
        //    }
        //}
    }
}
