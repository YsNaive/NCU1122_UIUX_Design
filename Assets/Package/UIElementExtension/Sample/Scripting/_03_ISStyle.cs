using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI.UITK.Sample
{
    /* #1 Intro */
    /*-----------------------------------------------------------------------------------------------------|
    | RSStyle is most useful class in RSStyle Group, because it contains all of others                     |
    | RSStyle doesn't have any specific properties, flag is use to define Enable/Disable for type of style |
    | For example: you can only turn on RSSize in this RSStyle, than it will act like a RSSize,            |
    | or you can turn on both RSSIze and RSPosition and ... etc.                                           |
    |-----------------------------------------------------------------------------------------------------*/
    public class _03_RSStyle
    {
        /*---------------------------------------------------------------|
        | Different to other RSStyleComponent. In RSStyle:               |
        | if you unset a property, it will make it into null             |
        | if you   set a property, it will create a new instance if need |
        |---------------------------------------------------------------*/
        void BasicProperties()
        {
            // Notice that default SetUnsetFlag is 0
            // so any of RSStyleComponent will equal to null in default
            RSStyle style = new RSStyle();
            _ = style.Display; // is NULL

            // before you assign value on Style, you have make Component enable
            // there are many ways can do that:
            style.SetEnable(RSStyleFlag.Display, true); // set by Enum
            style.SetFlag(RSStyle.F_Display, true);     // set by flag
            style.Display = new RSDisplay();            // set by new()

            // Remember we only enable the Display, so other is still NULL
            _ = style.Display;
            _ = style.Position; // NULL
            _ = style.Size;     // NULL
            // ... etc.
            // you can use SetAll() to enable them in once
            style.SetAll();

            // Here shows all properties in RSStyle Group
            RSDisplay display = style.Display;
            _ = display.opacity;
            _ = display.display;
            _ = display.visibility;
            _ = display.overflow;

            RSPosition position = style.Position;
            _ = position.position;
            _ = position.left;
            _ = position.top;
            _ = position.right;
            _ = position.bottom;

            RSFlex flex = style.Flex;
            _ = flex.basis;
            _ = flex.shrink;
            _ = flex.grow;
            _ = flex.direction;
            _ = flex.wrap;

            RSAlign align = style.Align;
            _ = align.alignSelf;
            _ = align.alignItems;
            _ = align.justifyContent;

            RSSize size = style.Size;
            _ = size.width;
            _ = size.height;
            _ = size.minWidth;
            _ = size.minHeight;
            _ = size.maxWidth;
            _ = size.maxHeight;

            RSMargin margin = style.Margin;
            _ = margin.left;
            _ = margin.top;
            _ = margin.right;
            _ = margin.bottom;
            _ = margin.any;

            RSPadding padding = style.Padding;
            _ = padding.left;
            _ = padding.top;
            _ = padding.right;
            _ = padding.bottom;
            _ = padding.any;

            RSText text = style.Text;
            _ = text.fontAsset;
            _ = text.fontStyle;
            _ = text.size;
            _ = text.color;
            _ = text.anchor;
            _ = text.wrap;
            _ = text.letterSpacing;
            _ = text.wordSpacing;
            _ = text.paragraphSpacing;

            RSBackground background = style.Background;
            _ = background.color;
            _ = background.background;
            _ = background.tintColor;
            _ = background.sliceLeft;
            _ = background.sliceTop;
            _ = background.sliceRight;
            _ = background.sliceBottom;

            RSBorder border = style.Border;
            _ = border.anyColor;
            _ = border.leftColor;
            _ = border.topColor;
            _ = border.rightColor;
            _ = border.bottomColor;
            _ = border.anyWidth;
            _ = border.leftWidth;
            _ = border.topWidth;
            _ = border.rightWidth;
            _ = border.bottomWidth;

            RSRadius radius = style.Radius;
            _ = radius.any;
            _ = radius.topLeft;
            _ = radius.bottomLeft;
            _ = radius.topRight;
            _ = radius.bottomRight;

            RSTransform transform = style.Transform;
            _ = transform.x;
            _ = transform.y;
            _ = transform.pivotX;
            _ = transform.pivotY;
            _ = transform.rotateDeg;
            _ = transform.scale;
        }
    }
}
