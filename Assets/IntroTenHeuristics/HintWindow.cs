using NaiveAPI.UITK;


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HintWindow : RSRuntimeWindow
{
    private RSTextElement hintElement;

    public HintWindow()
    {
        Dragable = false;
        Resizable = false;

        style.SetRS_Style(RSRadius.Pixel(30));
        style.alignItems = Align.Center;
        style.justifyContent = Justify.Center;
        contentContainer.style.justifyContent = Justify.Center;
        style.flexGrow = 1;

        hintElement = new RSTextElement();
        hintElement.style.unityTextAlign = TextAnchor.MiddleCenter;
        hintElement.style.fontSize = RSTheme.Current.MainText.size * 1.5f;
        Add(hintElement);
    }

    public static void Open(Rect rect, string hint)
    {
        HintWindow window = CreateWindow<HintWindow>();

        window.EnableTab = false;

        window.style.left = Length.Percent(rect.x);
        window.style.top = Length.Percent(rect.y);
        window.style.width = Length.Percent(rect.width);
        window.style.height = Length.Percent(rect.height);

        window.hintElement.text = hint;

        double time = Time.realtimeSinceStartupAsDouble;
        window.schedule.Execute(() =>
        {
            double diffMs = (Time.realtimeSinceStartupAsDouble - time) * 1000;
            
            window.style.opacity = (float)((1500 - diffMs) / 500);
        }).StartingIn(500).Every(100).Until(() =>
        {
            if ((Time.realtimeSinceStartupAsDouble - time) * 1000 > 1500)
            {
                window.Destory();
                return true;
            }

            return false;
        });
    }
}
