using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class ColorPicker : PopupElement
    {
        public enum Mode
        {
            HSV,
            RGB,
        }
        public static Texture2D HueTexture;
        static ColorPicker()
        {
            HueTexture = new Texture2D(256, 1);
            for(int i = 0; i < 256; i++)
                HueTexture.SetPixel(i, 0, Color.HSVToRGB(i / 256f, 1f, 1f));
            HueTexture.Apply();
        }
        public bool Dragable { get; set; }
        public event Action<Color> OnColorChanged;
        public readonly FloatRangeDrawer[] valueDrawers = new FloatRangeDrawer[4];
        public VisualElement squarePickingArea;
        public VisualElement squarePickingDot;
        VisualElement originColorDisplay;
        VisualElement newColorDisplay;
        public Color defaultColor
        {
            get => m_defaultColor;
            set
            {
                m_defaultColor = value;
                m_pickingHSV = value.ToHSV();
                m_pickingColor = value;
                originColorDisplay.MarkDirtyRepaint();
                _updatePickingColor();
            }
        }
        public Color m_defaultColor;
        private Color m_pickingColor;
        private (float h, float s, float v) m_pickingHSV;
        public Vector2 squarePickingPosition
        {
            set
            {
                var a = m_pickingColor.a;
                value /= squarePickingArea.contentRect.size;
                value.x = Mathf.Clamp01(value.x);
                value.y = Mathf.Clamp01(value.y);
                m_pickingHSV.s = value.x;
                m_pickingHSV.v = 1f-value.y;
                m_pickingColor = Color.HSVToRGB(m_pickingHSV.h, m_pickingHSV.s, m_pickingHSV.v);
                m_pickingColor.a = a;
                _updatePickingColor();
            }
        }
        static Texture2D squarePickingAreaTexture = null;
        public Color pickingColor
        {
            get => m_pickingColor;
            set
            {
                m_pickingColor = value;
                _updatePickingColor();
            }
        }
        public Mode mode
        {
            get => m_mode;
            set
            {
                if (m_mode == value) return;
                m_mode = value;
                switch (m_mode)
                {
                    case Mode.HSV:
                        SetModeToHSV();
                        break;
                    case Mode.RGB:
                        break;
                }
            }
        }
        private Mode m_mode;
        public ColorPicker() {
            RSRadius.op_temp.any = RSTheme.Current.LineHeight/2f;
            RSRadius.op_temp.ApplyOn(this);
            RSBorder.op_temp.anyWidth = 1f;
            RSBorder.op_temp.anyColor = RSTheme.Current.FrontgroundColor;
            RSBorder.op_temp.ApplyOn(this);
            RSPadding.op_temp.any = RSTheme.Current.VisualMargin;
            RSPadding.op_temp.ApplyOn(this);
            style.minWidth = RSTheme.Current.LabelWidth * 1.25f;
            style.minHeight = RSTheme.Current.LabelWidth * 2;
            style.backgroundColor = RSTheme.Current.BackgroundColor;
            var title = new RSLocalizeTextElement(RSLocalizeText.FromKey(RSLocalization.SR.colorPicker));
            title.style.backgroundColor = RSTheme.Current.BackgroundColor2;
            title.style.unityTextAlign = TextAnchor.MiddleLeft;
            title.style.paddingLeft = RSTheme.Current.VisualMargin;
            var cancelBtn = new RSButton("x");
            cancelBtn.style.marginLeft = StyleKeyword.Auto;
            cancelBtn.clicked += Close;
            title.Add(cancelBtn);
            title.AddManipulator(new DragManipulator(this));
            Add(title);


            var aboveLayoutContainer = new VisualElement();
            RSMargin.op_temp.any =RSTheme.Current.VisualMargin;
            RSMargin.op_temp.ApplyOn(aboveLayoutContainer);
            aboveLayoutContainer.style.marginLeft = RSTheme.Current.LineHeight;
            aboveLayoutContainer.style.flexDirection = FlexDirection.Row;
            var topRightContainer = new VisualElement();
            topRightContainer.style.flexGrow = 1f;
            originColorDisplay = new VisualElement();
            newColorDisplay = new VisualElement();
            squarePickingArea = new VisualElement();
            squarePickingDot = new VisualElement();
            squarePickingDot.style.position = Position.Absolute;
            squarePickingDot.style.width = Length.Percent(7);
            squarePickingDot.style.height = Length.Percent(7);
            squarePickingDot.style.left = Length.Percent(-3.5f);
            squarePickingDot.style.top = Length.Percent(-3.5f);
            RSRadius.op_temp.any = RSLength.Full;
            RSRadius.op_temp.ApplyOn(squarePickingDot);
            RSBorder.op_temp.anyWidth = RSLength.Percent(1f);
            RSBorder.op_temp.anyColor = Color.black;
            RSBorder.op_temp.ApplyOn(squarePickingDot);
            squarePickingArea.Add(squarePickingDot);
            squarePickingArea.AddManipulator(new SquarePickingManipulator(this));
            squarePickingDot.style.backgroundColor = Color.white;
            originColorDisplay.style.width = Length.Percent(100);
            originColorDisplay.style.height = Length.Percent(30);

            originColorDisplay.RegisterCallback<GeometryChangedEvent>(evt => { originColorDisplay.style.backgroundImage = ColorDrawer.getContainerBackground(evt.newRect.size); });
            newColorDisplay.RegisterCallback<GeometryChangedEvent>(evt => { newColorDisplay.style.backgroundImage = ColorDrawer.getContainerBackground(evt.newRect.size); });
            foreach (var item in new VisualElement[] { originColorDisplay, newColorDisplay })
            {
                item.style.width = Length.Percent(100);
                item.style.height = Length.Percent(25);
                item.style.marginLeft = RSTheme.Current.VisualMargin;
                item.style.marginBottom = RSTheme.Current.VisualMargin;
            }
            newColorDisplay.generateVisualContent += (MeshGenerationContext mgc) =>
            {
                var color = pickingColor;
                UIElementExtensionUtils.FillElementMeshGeneration(mgc, color, color, color, color);
            };
            originColorDisplay.generateVisualContent += (MeshGenerationContext mgc) => { UIElementExtensionUtils.FillElementMeshGeneration(mgc, defaultColor, defaultColor, defaultColor, defaultColor); };
            originColorDisplay.RegisterCallback<PointerDownEvent>(evt =>
            {
                evt.StopImmediatePropagation();
                pickingColor = defaultColor;
                _updatePickingColor();
            });
            topRightContainer.Add(originColorDisplay);
            topRightContainer.Add(newColorDisplay);
            aboveLayoutContainer.Add(squarePickingArea);
            aboveLayoutContainer.Add(topRightContainer);
            Add(aboveLayoutContainer);

            var attr = new RangeAttribute(0f, 1f);
            RSBorder.op_temp.anyWidth = 1f;
            RSBorder.op_temp.anyColor = Color.black;
            for (int i = 0; i < valueDrawers.Length; i++) 
            {
                int localIndex = i;
                var drawer = new FloatRangeDrawer();
                valueDrawers[localIndex] = drawer;
                drawer.labelWidth = RSTheme.Current.LineHeight;
                drawer.indentWidth = 0;
                drawer.slider.dragLineElement.style.height = Length.Percent(70);
                drawer.slider.dragLineElement.style.top = Length.Percent(15);
                drawer.OnValueChanged += _updateValue;
                drawer.slider.dragElement.style.SetRS_Style(RSBorder.op_temp);
                drawer.slider.dragElement.style.backgroundColor = Color.white;
                drawer.ReciveAttribute(attr);
                drawer.slider.dragLineElement.generateVisualContent += (MeshGenerationContext mgc) =>
                {
                    (var c1, var c2) = _getValueDrawerRenderColor(localIndex);
                    UIElementExtensionUtils.FillElementMeshGeneration(mgc, c1, c1, c2, c2);
                };
                Add(drawer);
            }
            valueDrawers[3].label = "A";
            valueDrawers[3].slider.dragLineElement.RegisterCallback<GeometryChangedEvent>(evt =>
            {
                valueDrawers[3].slider.dragLineElement.style.backgroundImage = ColorDrawer.getContainerBackground(evt.newRect.size);
                squarePickingArea.style.width = evt.newRect.width;
                squarePickingArea.style.height = evt.newRect.width;
            });
            mode = Mode.HSV;
            SetModeToHSV();
        }
        (Color c1, Color c2) _getValueDrawerRenderColor(int i)
        {
            var curColor = pickingColor;
            if (i == 3) return (curColor.NewA(0f), curColor.NewA(1f));
            switch (mode)
            {
                case Mode.HSV:
                    if (i == 1) return (Color.HSVToRGB(valueDrawers[0].value, 0f, valueDrawers[2].value),
                                        Color.HSVToRGB(valueDrawers[0].value, 1f, valueDrawers[2].value));
                    if (i == 2) return (Color.HSVToRGB(valueDrawers[0].value, valueDrawers[1].value, 0f),
                                        Color.HSVToRGB(valueDrawers[0].value, valueDrawers[1].value, 1f));
                    break;
                case Mode.RGB:
                    if (i == 0) return (curColor.NewR(0f), curColor.NewR(1f));
                    if (i == 1) return (curColor.NewG(0f), curColor.NewG(1f));
                    if (i == 2) return (curColor.NewB(0f), curColor.NewB(1f));
                    break;
            }
            return (Color.clear, Color.clear);
        }
        void _updateValue()
        {
            Color color;
            switch (mode)
            {
                case Mode.HSV:
                    color = Color.HSVToRGB(valueDrawers[0].value, valueDrawers[1].value, valueDrawers[2].value);
                    color.a = valueDrawers[3].value;
                    m_pickingHSV.h = valueDrawers[0].value;
                    m_pickingHSV.s = valueDrawers[1].value;
                    m_pickingHSV.v = valueDrawers[2].value;
                    m_pickingColor = color;
                    break;
                case Mode.RGB:
                    color = new Color(valueDrawers[0].value, valueDrawers[1].value, valueDrawers[2].value, valueDrawers[3].value);
                    var newHsv = color.ToHSV();
                    if(newHsv.v == 0)
                        newHsv.h = m_pickingHSV.h;
                    m_pickingHSV = newHsv;
                    m_pickingColor = color;
                    break;
            }
            _updatePickingColor();
        }
        void _updatePickingDrawer()
        {
            valueDrawers[3].value = m_pickingColor.a;
            switch (m_mode)
            {
                case Mode.HSV:
                    valueDrawers[0].SetValueWithoutNotify(m_pickingHSV.h);
                    valueDrawers[1].SetValueWithoutNotify(m_pickingHSV.s);
                    valueDrawers[2].SetValueWithoutNotify(m_pickingHSV.v);
                    break;
                case Mode.RGB:
                    valueDrawers[0].SetValueWithoutNotify(m_pickingColor.r);
                    valueDrawers[1].SetValueWithoutNotify(m_pickingColor.g);
                    valueDrawers[2].SetValueWithoutNotify(m_pickingColor.b);
                    break;
            }

            for (int i = 0; i < valueDrawers.Length; i++)
                valueDrawers[i].slider.dragLineElement.MarkDirtyRepaint();
            newColorDisplay.MarkDirtyRepaint();
        }
        void _updatePickingSquare()
        {
            squarePickingAreaTexture ??= new Texture2D(32, 32) { wrapMode = TextureWrapMode.Clamp };
            for (int i = 0, imax = squarePickingAreaTexture.width; i < imax; i++)
            {
                for (int j = 0, jmax = squarePickingAreaTexture.height; j < jmax; j++)
                    squarePickingAreaTexture.SetPixel(i, j, Color.HSVToRGB(valueDrawers[0].value, i / (imax - 1f), j / (jmax - 1f)));
            }
            squarePickingAreaTexture.Apply();
            squarePickingArea.style.backgroundImage = Background.FromTexture2D(squarePickingAreaTexture);

            var newPos = new Vector3(m_pickingHSV.s, 1f - m_pickingHSV.v) * squarePickingArea.contentRect.size;
            squarePickingDot.transform.position = newPos;

            RSBorder.op_temp.UnsetAll();
            RSBorder.op_temp.anyColor = Color.HSVToRGB(1f - m_pickingHSV.h, 1f - m_pickingHSV.s, 1f - m_pickingHSV.v);
            RSBorder.op_temp.ApplyOn(squarePickingDot);
        }
        void _updatePickingColor()
        {
            _updatePickingSquare();
            _updatePickingDrawer();
            OnColorChanged?.Invoke(m_pickingColor);
        }
        public void SetModeToHSV()
        {
            valueDrawers[0].label = "H";
            valueDrawers[1].label = "S";
            valueDrawers[2].label = "V";
            valueDrawers[0].slider.dragLineElement.style.backgroundImage = Background.FromTexture2D(HueTexture);
            //valueDrawers[1].slider.dragLineElement.style.backgroundImage = Background.FromTexture2D(drawerTexture[1]);
        }






        class DragManipulator : PointerManipulator
        {
            public bool IsCapturing { get; private set; }
            private Vector2 coordOffset;
            ColorPicker dragTarget;
            public DragManipulator(ColorPicker dragTarget)
            {
                this.dragTarget = dragTarget;
            }

            private void PointerDownEvent(PointerDownEvent evt)
            {
                evt.StopPropagation();
                target.CapturePointer(evt.pointerId);
                IsCapturing = true;
                var pos = new Vector2(dragTarget.style.left.value.value, dragTarget.style.top.value.value);
                pos = dragTarget.parent.LocalToWorld(pos);
                coordOffset = (Vector2)evt.position - pos;
            }
            private void PointerMoveEvent(PointerMoveEvent evt)
            {
                if (!IsCapturing) return;
                if (!dragTarget.Dragable) return;
                var pos = dragTarget.parent.WorldToLocal((Vector2)evt.position - coordOffset);
                dragTarget.style.left = pos.x;
                dragTarget.style.top = pos.y;
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
        class SquarePickingManipulator: PointerManipulator
        {
            public bool IsCapturing { get; private set; }
            private Vector2 coordOffset;
            ColorPicker picker;
            public SquarePickingManipulator(ColorPicker picker)
            {
                this.picker = picker;
            }

            private void PointerDownEvent(PointerDownEvent evt)
            {
                evt.StopPropagation();
                target.CapturePointer(evt.pointerId);
                IsCapturing = true;
                picker.squarePickingPosition = evt.localPosition;
            }
            private void PointerMoveEvent(PointerMoveEvent evt)
            {
                if (!IsCapturing) return;
                picker.squarePickingPosition = evt.localPosition;
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