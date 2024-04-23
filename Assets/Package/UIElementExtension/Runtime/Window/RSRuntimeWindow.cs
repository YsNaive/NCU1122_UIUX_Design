using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSRuntimeWindow : RuntimeWindow
    {
        public RSRuntimeWindow()
        {
            style.backgroundColor = RSTheme.Current.BackgroundColor;
            style.SetRS_Style(new RSBorder(RSTheme.Current.BackgroundColor, 2));
            contentContainer.style.SetRS_Style(new RSPadding { any = 7 });
            RSTheme.Current.ApplyTextStyle(TabElement);
            TabElement.style.backgroundColor = RSTheme.Current.BackgroundColor2;


            int resizablePx = 4;
            this.AddManipulator(new ResizeManipulator(
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
                        style.borderLeftColor = RSTheme.Current.FrontgroundColor;
                    else if (x == 2)
                        style.borderRightColor = RSTheme.Current.FrontgroundColor;

                    if (y == 0)
                        style.borderTopColor = RSTheme.Current.FrontgroundColor;
                    else if (y == 2)
                        style.borderBottomColor = RSTheme.Current.FrontgroundColor;
                },
                leaveAnchor =>
                {
                    style.borderLeftColor = RSTheme.Current.BackgroundColor2;
                    style.borderRightColor = RSTheme.Current.BackgroundColor2;
                    style.borderTopColor = RSTheme.Current.BackgroundColor2;
                    style.borderBottomColor = RSTheme.Current.BackgroundColor2;
                }
                ));
        }
    }
}