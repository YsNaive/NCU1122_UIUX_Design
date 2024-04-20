using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSScrollView : ScrollView
    {
        public new class UxmlFactory : UxmlFactory<RSScrollView, UxmlTraits> { }
        public new class UxmlTraits : ScrollView.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ScrollView scroller = (ScrollView)ve;
                ApplyStyle(scroller);
            }
        }
        public RSScrollView()
        {
            contentContainer.style.width = StyleKeyword.Auto;
            style.minHeight = RSTheme.Current.LineHeight;
            ApplyStyle(this);
        }
        public static void ApplyStyle(ScrollView scrollView)
        {
            scrollView.style.ClearMarginPadding();
            RSScroller.ApplyStyle(scrollView.verticalScroller);
            RSScroller.ApplyStyle(scrollView.horizontalScroller);
        }
    }

}