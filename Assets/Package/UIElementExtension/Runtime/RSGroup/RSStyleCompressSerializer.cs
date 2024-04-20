using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public class RSStyleCompressSerializer : ISerializationCallbackReceiver
    {
        [System.NonSerialized]
        public List<RSStyleComponent> StyleComponents;
        [SerializeField] public List<int> s_data;
        [SerializeField] public List<Object> s_objData;

        public void Zip()
        {
            valueInit();
            s_data ??= new();
            s_objData ??= new();
            s_data.Clear();
            s_objData.Clear();
            foreach(var com in StyleComponents)
                addComponent(com);
        }
        public void UnZip()
        {
            valueInit();
            StyleComponents ??= new();
            StyleComponents.Clear();
            while (currentIndex < s_data.Count)
            {
                var obj = takeComponent();
                StyleComponents.Add(obj);
            }
        }

        private int currentIndex = 0;
        private int currentObjIndex = 0;
        private void valueInit()
        {
            currentIndex = 0;
            currentObjIndex = 0;
        }

        private void addData(Object value)
        {
            s_objData.Add(value);
        }
        private Object takeObject()
        {
            return s_objData[currentObjIndex++];
        }
        private void addData(Color value)
        {
            s_data.Add(value.ToU32());
        }
        private Color takeColor()
        {
            Color c = new();
            c.FromU32(takeInt());
            return c;
        }
        private void addData(Enum value)
        {
            s_data.Add(Convert.ToInt32(value));
        }
        private void addData(float value)
        {
            s_data.Add((int)(value*100));
        }
        private float takeFloat()
        {
            return s_data[currentIndex++] / 100f;
        }
        private void addData(int value)
        {
            s_data.Add(value);
        }
        private int takeInt()
        {
            return s_data[currentIndex++];
        }
        private void addData(RSLength length)
        {
            s_data.Add((int)length.Mode);
            s_data.Add((int)(length.value * 100));
        }
        private RSLength takeRSLength()
        {
            RSLength ret = new();
            ret.SetModeFlag((RSLength.ModeFlag)s_data[currentIndex++]);
            ret.value = s_data[currentIndex++]/100f;
            return ret;
        }
        private void addComponent(RSStyleComponent component)
        {
            addData(component.StyleFlag);
            addData(component.SetUnsetFlag);
            switch (component)
            {
                case RSDisplay display:
                    if (display.GetFlag(RSDisplay.F_Opacity))    addData(display.opacity);
                    if (display.GetFlag(RSDisplay.F_Display))    addData(display.display);
                    if (display.GetFlag(RSDisplay.F_Visibility)) addData(display.visibility);
                    if (display.GetFlag(RSDisplay.F_Overflow))   addData(display.overflow);
                    break;
                case RSPosition position:
                    if (position.GetFlag(RSPosition.F_Position)) addData(position.position);
                    if (position.GetFlag(RSPosition.F_Left))     addData(position.left);
                    if (position.GetFlag(RSPosition.F_Top))      addData(position.top);
                    if (position.GetFlag(RSPosition.F_Right))    addData(position.right);
                    if (position.GetFlag(RSPosition.F_Bottom))   addData(position.bottom);
                    break;
                case RSFlex flex:
                    if (flex.GetFlag(RSFlex.F_Basis))  addData(flex.basis);
                    if (flex.GetFlag(RSFlex.F_Shrink)) addData(flex.shrink);
                    if (flex.GetFlag(RSFlex.F_Grow))   addData(flex.grow);
                    if (flex.GetFlag(RSFlex.F_Direction)) addData(flex.direction);
                    if (flex.GetFlag(RSFlex.F_Wrap))   addData(flex.wrap);
                    break;
                case RSAlign align:
                    if (align.GetFlag(RSAlign.F_AlignSelf))      addData(align.alignSelf);
                    if (align.GetFlag(RSAlign.F_AlignItems))     addData(align.alignItems);
                    if (align.GetFlag(RSAlign.F_JustifyContent)) addData(align.justifyContent);
                    break;
                case RSSize size:
                    if (size.GetFlag(RSSize.F_Width))     addData(size.width);
                    if (size.GetFlag(RSSize.F_MinWidth))  addData(size.minWidth);
                    if (size.GetFlag(RSSize.F_MaxWidth))  addData(size.maxWidth);
                    if (size.GetFlag(RSSize.F_Height))    addData(size.height);
                    if (size.GetFlag(RSSize.F_MinHeight)) addData(size.minHeight);
                    if (size.GetFlag(RSSize.F_MaxHeight)) addData(size.maxHeight);
                    break;
                case RSMargin margin:
                    if (margin.GetFlag(RSMargin.F_Left))   addData(margin.left);
                    if (margin.GetFlag(RSMargin.F_Top))    addData(margin.top);
                    if (margin.GetFlag(RSMargin.F_Right))  addData(margin.right);
                    if (margin.GetFlag(RSMargin.F_Bottom)) addData(margin.bottom);
                    break;
                case RSPadding padding:
                    if (padding.GetFlag(RSPadding.F_Left))   addData(padding.left);
                    if (padding.GetFlag(RSPadding.F_Top))    addData(padding.top);
                    if (padding.GetFlag(RSPadding.F_Right))  addData(padding.right);
                    if (padding.GetFlag(RSPadding.F_Bottom)) addData(padding.bottom);
                    break;
                case RSText text:
                    if (text.GetFlag(RSText.F_FontAsset)) addData(text.fontAsset);
                    if (text.GetFlag(RSText.F_FontStyle)) addData(text.fontStyle);
                    if (text.GetFlag(RSText.F_Size))      addData(text.size);
                    if (text.GetFlag(RSText.F_Color))     addData(text.color);
                    if (text.GetFlag(RSText.F_Anchor))    addData(text.anchor);
                    if (text.GetFlag(RSText.F_Wrap))      addData(text.wrap);
                    if (text.GetFlag(RSText.F_LetterSpacing))    addData(text.letterSpacing);
                    if (text.GetFlag(RSText.F_WordSpacing))      addData(text.wordSpacing);
                    if (text.GetFlag(RSText.F_ParagraphSpacing)) addData(text.paragraphSpacing);
                    break;
                case RSBackground background:
                    if (background.GetFlag(RSBackground.F_Color))       addData(background.color);
                    if (background.GetFlag(RSBackground.F_Background))  addData(RSBackground.BackgroundToObject(background.background));
                    if (background.GetFlag(RSBackground.F_TintColor))   addData(background.tintColor);
                    if (background.GetFlag(RSBackground.F_SliceLeft))   addData(background.sliceLeft);
                    if (background.GetFlag(RSBackground.F_SliceTop))    addData(background.sliceTop);
                    if (background.GetFlag(RSBackground.F_SliceRight))  addData(background.sliceRight);
                    if (background.GetFlag(RSBackground.F_SliceBottom)) addData(background.sliceBottom);
                    break;
                case RSBorder border:
                    if (border.GetFlag(RSBorder.F_LeftColor))   addData(border.leftColor);
                    if (border.GetFlag(RSBorder.F_TopColor))    addData(border.topColor);
                    if (border.GetFlag(RSBorder.F_RightColor))  addData(border.rightColor);
                    if (border.GetFlag(RSBorder.F_BottomColor)) addData(border.bottomColor);
                    if (border.GetFlag(RSBorder.F_LeftWidth))   addData(border.leftWidth);
                    if (border.GetFlag(RSBorder.F_TopWidth))    addData(border.topWidth);
                    if (border.GetFlag(RSBorder.F_RightWidth))  addData(border.rightWidth);
                    if (border.GetFlag(RSBorder.F_BottomWidth)) addData(border.bottomWidth);
                    break;
                case RSRadius radius:
                    if (radius.GetFlag(RSRadius.F_TopLeft))     addData(radius.topLeft);
                    if (radius.GetFlag(RSRadius.F_BottomLeft))  addData(radius.bottomLeft);
                    if (radius.GetFlag(RSRadius.F_TopRight))    addData(radius.topRight);
                    if (radius.GetFlag(RSRadius.F_BottomRight)) addData(radius.bottomRight);
                    break;
                case RSTransform transform:
                    if (transform.GetFlag(RSTransform.F_Pivot))
                    {
                        addData(transform.pivotX);
                        addData(transform.pivotY);
                    }
                    if (transform.GetFlag(RSTransform.F_Position))
                    {
                        addData(transform.x);
                        addData(transform.y);
                    }
                    if (transform.GetFlag(RSTransform.F_Scale))
                    {
                        addData(transform.scale.x);
                        addData(transform.scale.y);
                    }
                    if (transform.GetFlag(RSTransform.F_RotateDeg)) addData(transform.rotateDeg);
                    break;
                case RSStyle style:
                    foreach(var com in style.VisitActiveStyle())
                        addComponent(com);
                    break;
            }
        }
        private RSStyleComponent takeComponent()
        {
            RSStyleFlag styleFlag = (RSStyleFlag)takeInt();
            int setFlag = takeInt();
            switch (styleFlag)
            {
                case RSStyleFlag.Display:
                    RSDisplay display = new RSDisplay();
                    display.SetUnsetFlag = setFlag;
                    if (display.GetFlag(RSDisplay.F_Opacity))    display.opacity    = takeFloat();
                    if (display.GetFlag(RSDisplay.F_Display))    display.display    = (DisplayStyle)takeInt();
                    if (display.GetFlag(RSDisplay.F_Visibility)) display.visibility = (Visibility)takeInt();
                    if (display.GetFlag(RSDisplay.F_Overflow))   display.overflow   = (Overflow)takeInt();
                    return display;

                case RSStyleFlag.Position:
                    RSPosition position = new RSPosition();
                    position.SetUnsetFlag = setFlag;
                    if (position.GetFlag(RSPosition.F_Position)) position.position = (Position)takeInt();
                    if (position.GetFlag(RSPosition.F_Left))     position.left   = takeRSLength();
                    if (position.GetFlag(RSPosition.F_Top))      position.top    = takeRSLength();
                    if (position.GetFlag(RSPosition.F_Right))    position.right  = takeRSLength();
                    if (position.GetFlag(RSPosition.F_Bottom))   position.bottom = takeRSLength();
                    return position;

                case RSStyleFlag.Flex:
                    RSFlex flex = new RSFlex();
                    flex.SetUnsetFlag = setFlag;
                    if (flex.GetFlag(RSFlex.F_Basis))  flex.basis = takeRSLength();
                    if (flex.GetFlag(RSFlex.F_Shrink)) flex.shrink = takeFloat();
                    if (flex.GetFlag(RSFlex.F_Grow))   flex.grow = takeFloat();
                    if (flex.GetFlag(RSFlex.F_Direction)) flex.direction = (FlexDirection)takeInt();
                    if (flex.GetFlag(RSFlex.F_Wrap))   flex.wrap = (Wrap)takeInt();
                    return flex;

                case RSStyleFlag.Align:
                    RSAlign align = new RSAlign();
                    align.SetUnsetFlag = setFlag;
                    if (align.GetFlag(RSAlign.F_AlignSelf))      align.alignSelf = (Align)takeInt();
                    if (align.GetFlag(RSAlign.F_AlignItems))     align.alignItems = (Align)takeInt();
                    if (align.GetFlag(RSAlign.F_JustifyContent)) align.justifyContent = (Justify)takeInt();
                    return align;

                case RSStyleFlag.Size:
                    RSSize size = new RSSize();
                    size.SetUnsetFlag = setFlag;
                    if (size.GetFlag(RSSize.F_Width))     size.width     = takeRSLength();
                    if (size.GetFlag(RSSize.F_MinWidth))  size.minWidth  = takeRSLength();
                    if (size.GetFlag(RSSize.F_MaxWidth))  size.maxWidth  = takeRSLength();
                    if (size.GetFlag(RSSize.F_Height))    size.height    = takeRSLength();
                    if (size.GetFlag(RSSize.F_MinHeight)) size.minHeight = takeRSLength();
                    if (size.GetFlag(RSSize.F_MaxHeight)) size.maxHeight = takeRSLength();
                    return size;

                case RSStyleFlag.Margin:
                    RSMargin margin = new RSMargin();
                    margin.SetUnsetFlag = setFlag;
                    if (margin.GetFlag(RSMargin.F_Left))   margin.left   = takeRSLength();
                    if (margin.GetFlag(RSMargin.F_Top))    margin.top    = takeRSLength();
                    if (margin.GetFlag(RSMargin.F_Right))  margin.right  = takeRSLength();
                    if (margin.GetFlag(RSMargin.F_Bottom)) margin.bottom = takeRSLength();
                    return margin;

                case RSStyleFlag.Padding:
                    RSPadding padding = new RSPadding();
                    padding.SetUnsetFlag = setFlag;
                    if (padding.GetFlag(RSMargin.F_Left))   padding.left   = takeRSLength();
                    if (padding.GetFlag(RSMargin.F_Top))    padding.top    = takeRSLength();
                    if (padding.GetFlag(RSMargin.F_Right))  padding.right  = takeRSLength();
                    if (padding.GetFlag(RSMargin.F_Bottom)) padding.bottom = takeRSLength();
                    return padding;

                case RSStyleFlag.Text:
                    RSText text = new RSText();
                    text.SetUnsetFlag = setFlag;
                    if (text.GetFlag(RSText.F_FontAsset)) text.fontAsset = takeObject() as FontAsset;
                    if (text.GetFlag(RSText.F_FontStyle)) text.fontStyle = (FontStyle)takeInt();
                    if (text.GetFlag(RSText.F_Size))      text.size      = takeRSLength();
                    if (text.GetFlag(RSText.F_Color))     text.color     = takeColor();
                    if (text.GetFlag(RSText.F_Anchor))    text.anchor    = (TextAnchor)takeInt();
                    if (text.GetFlag(RSText.F_Wrap))      text.wrap      = (WhiteSpace)takeInt();
                    if (text.GetFlag(RSText.F_LetterSpacing))    text.letterSpacing    = takeRSLength();
                    if (text.GetFlag(RSText.F_WordSpacing))      text.wordSpacing      = takeRSLength();
                    if (text.GetFlag(RSText.F_ParagraphSpacing)) text.paragraphSpacing = takeRSLength();
                    return text;

                case RSStyleFlag.Background:
                    RSBackground background = new RSBackground();
                    background.SetUnsetFlag = setFlag;
                    if (background.GetFlag(RSBackground.F_Color))       background.color = takeColor();
                    if (background.GetFlag(RSBackground.F_Background))  background.background = RSBackground.ObjectToBackground(takeObject());
                    if (background.GetFlag(RSBackground.F_TintColor))   background.tintColor = takeColor();
                    if (background.GetFlag(RSBackground.F_SliceLeft))   background.sliceLeft = takeInt();
                    if (background.GetFlag(RSBackground.F_SliceTop))    background.sliceTop = takeInt();
                    if (background.GetFlag(RSBackground.F_SliceRight))  background.sliceRight = takeInt();
                    if (background.GetFlag(RSBackground.F_SliceBottom)) background.sliceBottom = takeInt();
                    return background;

                case RSStyleFlag.Border:
                    RSBorder border = new RSBorder();
                    border.SetUnsetFlag = setFlag;
                    if (border.GetFlag(RSBorder.F_LeftColor))   border.leftColor = takeColor();
                    if (border.GetFlag(RSBorder.F_TopColor))    border.topColor = takeColor();
                    if (border.GetFlag(RSBorder.F_RightColor))  border.rightColor = takeColor();
                    if (border.GetFlag(RSBorder.F_BottomColor)) border.bottomColor = takeColor();
                    if (border.GetFlag(RSBorder.F_LeftWidth))   border.leftWidth = takeFloat();
                    if (border.GetFlag(RSBorder.F_TopWidth))    border.topWidth = takeFloat();
                    if (border.GetFlag(RSBorder.F_RightWidth))  border.rightWidth = takeFloat();
                    if (border.GetFlag(RSBorder.F_BottomWidth)) border.bottomWidth = takeFloat();
                    return border;

                case RSStyleFlag.Radius:
                    RSRadius radius = new RSRadius();
                    radius.SetUnsetFlag = setFlag;
                    if (radius.GetFlag(RSRadius.F_TopLeft))     radius.topLeft = takeRSLength();
                    if (radius.GetFlag(RSRadius.F_BottomLeft))  radius.bottomLeft = takeRSLength();
                    if (radius.GetFlag(RSRadius.F_TopRight))    radius.topRight = takeRSLength();
                    if (radius.GetFlag(RSRadius.F_BottomRight)) radius.bottomRight = takeRSLength();
                    return radius;

                case RSStyleFlag.Transform:
                    RSTransform transform = new RSTransform();
                    transform.SetUnsetFlag = setFlag;
                    if (transform.GetFlag(RSTransform.F_Pivot))
                    {
                        transform.pivotX = takeRSLength();
                        transform.pivotY = takeRSLength();
                    }
                    if (transform.GetFlag(RSTransform.F_Position))
                    {
                        transform.x = takeRSLength();
                        transform.y = takeRSLength();
                    }
                    if (transform.GetFlag(RSTransform.F_Scale))
                    {
                        transform.scale = new Vector2(takeFloat(), takeFloat());
                    }
                    if (transform.GetFlag(RSTransform.F_RotateDeg)) transform.rotateDeg = takeFloat();
                    return transform;

                case RSStyleFlag.AllStyle:
                    RSStyle style = new RSStyle();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Display, setFlag))
                        style.Display = (RSDisplay)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Position, setFlag))
                        style.Position = (RSPosition)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Flex, setFlag))
                        style.Flex = (RSFlex)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Align, setFlag))
                        style.Align = (RSAlign)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Size, setFlag))
                        style.Size = (RSSize)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Margin, setFlag))
                        style.Margin = (RSMargin)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Padding, setFlag))
                        style.Padding = (RSPadding)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Text, setFlag))
                        style.Text = (RSText)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Background, setFlag))
                        style.Background = (RSBackground)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Border, setFlag))
                        style.Border = (RSBorder)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Radius, setFlag))
                        style.Radius = (RSRadius)takeComponent();
                    if (RSStyleComponent.GetFlag((int)RSStyleFlag.Transform, setFlag))
                        style.Transform = (RSTransform)takeComponent();
                    return style;
            }
            return null;
        }

        public void OnBeforeSerialize()
        {
            Zip();
        }

        public void OnAfterDeserialize()
        {
            UnZip();
            s_data = null;
            s_objData = null;
        }
    }
}
