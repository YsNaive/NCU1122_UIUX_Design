using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UserDataPopupWindow : RuntimeWindow
{
    VisualElement dataContainer;
    public UserDataPopupWindow()
    {
        Dragable = false;
        EnableTab = false;
        Resizable = false;
        InitLayoutAsPercent(new Rect(0, 0, 1, 1));
        Color targetBG = Color.black.NewA(0.5f);
        style.backgroundColor = Color.clear;
        dataContainer = new VisualElement();
        RSBorder.op_temp.anyWidth = 1f;
        RSBorder.op_temp.anyColor = RSTheme.Current.FrontgroundColor;
        RSRadius.op_temp.any = RSTheme.Current.LineHeight / 2f;
        RSBorder.op_temp.ApplyOn(dataContainer);
        RSRadius.op_temp.ApplyOn(dataContainer);
        dataContainer.style.backgroundColor = RSTheme.Current.BackgroundColor;
        var containerWidth = RSTheme.Current.LabelWidth * 2f;
        dataContainer.style.width = containerWidth;
        dataContainer.style.translate = new Translate(containerWidth, 0);
        dataContainer.style.height = Length.Percent(98);
        RSMargin.op_temp.any = Length.Percent(1);
        RSMargin.op_temp.ApplyOn(dataContainer);
        RSPadding.op_temp.any = 10;
        RSPadding.op_temp.ApplyOn(dataContainer);
        dataContainer.style.marginLeft = StyleKeyword.Auto;
        Add(dataContainer);
        var popupItem = dataContainer.schedule.Execute(() =>
        {
            var pos = dataContainer.transform.position;
            pos.x *= 0.9f;
            var rate = pos.x / containerWidth;
            style.backgroundColor = targetBG * (1f - rate) + Color.clear * rate;
            if (pos.x < 1) pos.x = 0;
            dataContainer.transform.position = pos;
        }).Every(10).Until(() => dataContainer.transform.position.x == 0);

        RegisterCallback<PointerDownEvent>(evt =>
        {
            popupItem.Pause();
            evt.StopImmediatePropagation();
            dataContainer.schedule.Execute(() =>
            {
                var pos = dataContainer.transform.position;
                pos.x *= 1.08f;
                var rate = pos.x / containerWidth;
                style.backgroundColor = targetBG * (1f - rate) + Color.clear * rate;
                pos.x++;
                dataContainer.transform.position = pos;
            }).Every(10).Until(() =>
            {
                if(dataContainer.transform.position.x > containerWidth)
                {
                    Destory();
                    return true;
                }
                return false;
            });
        });
        dataContainer.transform.position = new Vector2(dataContainer.style.width.value.value, 0);
    }
    public static void Open(UserData data)
    {
        var window = CreateWindow<UserDataPopupWindow>();
        var hor = new RSHorizontal();
        var basicInfoContainer = new VisualElement();
        basicInfoContainer.style.marginLeft = 10;
        var icon = new VisualElement()
        {
            style =
            {
                width= 120,
                height = 120,
                backgroundImage = data.IconTexture,
            }
        };
        RSRadius.op_temp.any = RSTheme.Current.LineHeight / 3f;
        RSRadius.op_temp.ApplyOn(icon);
        RSBorder.op_temp.anyWidth = 1f;
        RSBorder.op_temp.anyColor = RSTheme.Current.BackgroundColor2;
        RSBorder.op_temp.ApplyOn(icon);

        basicInfoContainer.Add(new RSTextElement($"Major : {data.Major}"));
        basicInfoContainer.Add(new RSTextElement($"Career : {data.Career}"));
        basicInfoContainer.Add(new RSTextElement($"Gender : {data.Gender}"));


        hor.Add(icon);
        hor.Add(basicInfoContainer);
        window.dataContainer.Add(hor);

    }
}
