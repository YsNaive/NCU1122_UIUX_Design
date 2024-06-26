using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSHorizontal : VisualElement
    {
        public RSHorizontal() : this(0f, false) { }
        public RSHorizontal(params VisualElement[] elements) : this(0f, false, elements) { }
        public RSHorizontal(float spacePercent, params VisualElement[] elements) : this(spacePercent, false, elements) { }
        public RSHorizontal(float spacePercent, bool isSpaceBetween, params VisualElement[] elements)
        {
            style.flexDirection = FlexDirection.Row;
            style.minHeight = RSTheme.Current.LineHeight;

            if (elements.Length == 0) return;
            float width = (100f / elements.Length);
            VisualElement last = null;
            foreach (var ve in elements)
            {
                if (ve != null)
                {
                    ve.style.width = Length.Percent(width - spacePercent);
                    if (isSpaceBetween)
                    {
                        ve.style.marginRight = Length.Percent(spacePercent / 2f);
                        ve.style.marginLeft = Length.Percent(spacePercent / 2f);
                    }
                    else
                    {
                        ve.style.marginLeft = Length.Percent(spacePercent);
                    }
                    last = ve;
                    Add(ve);
                }
                else
                {
                    if (last == null) continue;
                    last.style.width = Length.Percent(last.style.width.value.value + width);
                }
            }
        }
    }
}
