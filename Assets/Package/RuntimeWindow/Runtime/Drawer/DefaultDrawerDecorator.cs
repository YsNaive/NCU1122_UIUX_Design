using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    public class DefaultDrawerDecorator
    {
        public class TooltipDecorator : IRuntimeDrawerDecorator
        {
            public Type RequiredAttribute => typeof(TooltipAttribute);
            private static PopupElement popup;
            private static INotifyValueChanged<string> popupText;
            static TooltipDecorator()
            {
                popup = new PopupElement();
                popup.style.backgroundColor = DocStyle.Current.BackgroundColor;
                popup.style.SetIS_Style(new ISBorder(DocStyle.Current.FrontgroundColor, 1f));
                popup.pickingMode = PickingMode.Ignore;
                popup.CoverMask.pickingMode = PickingMode.Ignore;
                var text = new DSTextElement();
                text.style.paddingLeft = DocStyle.Current.MainTextSize / 2;
                text.style.paddingRight = DocStyle.Current.MainTextSize / 2;
                popupText = text;
                popup.Add(text);
            }
            public void DecorateDrawer(Attribute attribute, RuntimeDrawer drawer)
            {
                TooltipAttribute tooltip = (TooltipAttribute)attribute;
                bool isHover = false;
                drawer.titleElement.RegisterCallback<PointerEnterEvent>(evt =>
                {
                    isHover = true;
                    popup.Open(drawer);
                    popup.visible = false;
                    popupText.SetValueWithoutNotify(tooltip.tooltip);
                    drawer.schedule.Execute(() =>
                    {
                        if (!isHover) return;
                        popup.visible = true;
                        var pos = drawer.labelElement.LocalToWorld(Vector2.zero);
                        pos = popup.CoverMask.WorldToLocal(pos);
                        pos.y -= popup.localBound.height;
                        pos.x -= DocStyle.Current.MainTextSize;
                        popup.transform.position = pos;
                    }).ExecuteLater(500);
                });
                drawer.titleElement.RegisterCallback<PointerLeaveEvent>(evt =>
                {
                    isHover = false;
                    popup.Close();
                });
            }
        }
        public class SpaceDecorator : IRuntimeDrawerDecorator
        {
            public Type RequiredAttribute => typeof(SpaceAttribute);

            public void DecorateDrawer(Attribute attribute, RuntimeDrawer drawer)
            {
                drawer.style.marginTop = ((SpaceAttribute)attribute).height;
            }
        }
    }
}