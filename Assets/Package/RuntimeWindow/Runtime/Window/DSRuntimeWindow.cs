using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    public class DSRuntimeWindow : RuntimeWindow
    {
        public DSRuntimeWindow()
        {
            style.backgroundColor = DocStyle.Current.BackgroundColor;
            style.SetIS_Style(new ISBorder(DocStyle.Current.SubBackgroundColor, 2));
            contentContainer.style.SetIS_Style(ISPadding.Pixel(7));
            TabElement.style.SetIS_Style(DocStyle.Current.MainTextStyle);
            TabElement.style.backgroundColor = DocStyle.Current.SubBackgroundColor;


            int resizablePx = 4;
            this.AddManipulator(new PointerResizeManipulator(
                pos =>
                {
                    var bound = localBound;
                    int result = 0;
                    if (pos.x < resizablePx)
                        result += 0;
                    else if (pos.x > localBound.width - resizablePx)
                        result += 2;
                    else
                        result += 1;

                    if (pos.y < resizablePx)
                        result += 0;
                    else if (pos.y > localBound.height - resizablePx)
                        result += 6;
                    else
                        result += 3;

                    return (TextAnchor)result;
                },
                hoverAnchor =>
                {
                    int x = (int)hoverAnchor % 3;
                    int y = (int)hoverAnchor / 3;
                    if (x == 0)
                        style.borderLeftColor = DocStyle.Current.FrontgroundColor;
                    else if (x == 2)
                        style.borderRightColor = DocStyle.Current.FrontgroundColor;

                    if (y == 0)
                        style.borderTopColor = DocStyle.Current.FrontgroundColor;
                    else if (y == 2)
                        style.borderBottomColor = DocStyle.Current.FrontgroundColor;
                },
                leaveAnchor =>
                {
                    style.borderLeftColor = DocStyle.Current.SubBackgroundColor;
                    style.borderRightColor = DocStyle.Current.SubBackgroundColor;
                    style.borderTopColor = DocStyle.Current.SubBackgroundColor;
                    style.borderBottomColor = DocStyle.Current.SubBackgroundColor;
                }
                ));
        }
    }
}