using NaiveAPI.DocumentBuilder;
using NaiveAPI.RuntimeWindowUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DoubleCheckWindow : DSRuntimeWindow
{
    private DSLabel warningLabel;
    private DSButton btConfirm, btCancel;

    public DoubleCheckWindow()
    {
        EnableTab = false;
        EnableContextMenu = false;
        Resizable = false;
        Dragable = false;
        MinSize = new Vector2(600, 400);
        contentContainer.style.alignItems = Align.Center;
        contentContainer.style.justifyContent = Justify.Center;

        Color background = style.backgroundColor.value; background.a = 0.5f;
        style.backgroundColor = background;

        VisualElement container = new VisualElement();
        container.style.width = Length.Percent(60);
        container.style.height = Length.Percent(40);
        container.style.backgroundColor = DocStyle.Current.SubBackgroundColor;
        container.style.alignItems = Align.Center;
        container.style.justifyContent = Justify.Center;

        VisualElement visualElement = new VisualElement();
        visualElement.style.flexDirection = FlexDirection.Row;

        warningLabel = new DSLabel();

        btConfirm = new DSButton("�T�{", DocStyle.Current.SuccessColor);

        btCancel = new DSButton("����", DocStyle.Current.DangerColor);

        btConfirm.style.marginRight = 40;
        btCancel.style.marginLeft = 40;
        btConfirm.style.minWidth = Length.Percent(20);
        btCancel.style.minWidth = Length.Percent(20);

        visualElement.Add(btConfirm);

        visualElement.Add(btCancel);

        warningLabel.style.marginBottom = Length.Percent(5);
        visualElement.style.marginTop = Length.Percent(5);

        container.Add(warningLabel);
        container.Add(visualElement);

        Add(container);
    }

    public static void Open(Rect rect, string warning, Action<bool> callback)
    {
        DoubleCheckWindow window = CreateWindow<DoubleCheckWindow>();

        //window.style.left = Length.Percent(rect.x);
        //window.style.top = Length.Percent(rect.y);
        //window.style.width = Length.Percent(rect.width);
        //window.style.height = Length.Percent(rect.height);

        window.style.width = Length.Percent(100);
        window.style.height = Length.Percent(100);

        window.warningLabel.text = warning;

        window.btConfirm.clicked += () =>
        {
            callback(true);
            window.Destory();
        };

        window.btCancel.clicked += () =>
        {
            callback(false);
            window.Destory();
        };
    }
}
