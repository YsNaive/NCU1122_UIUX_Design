using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class TooltipDecorator : IRuntimeDrawerDecorator
    {
        public Type RequiredAttribute => typeof(TooltipAttribute);
        public void DecorateDrawer(Attribute attribute, RuntimeDrawer drawer)
        {
            drawer.tooltipElement.Add(new RSTextElement((attribute as TooltipAttribute).tooltip));
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