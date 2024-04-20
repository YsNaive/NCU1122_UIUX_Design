using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSScroller : Scroller
    {
        public new class UxmlFactory : UxmlFactory<RSScroller, UxmlTraits> { }
        public new class UxmlTraits : Scroller.UxmlTraits
        {
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ApplyStyle((Scroller)ve);
            }
        }
        public RSScroller()
        {
            ApplyStyle(this);
        }
        public new SliderDirection direction
        {
            get => base.direction;
            set
            {
                base.direction = value;
                ApplyDirection(this);
            }
        }
        public static void ApplyStyle(Scroller scroller)
        {
            scroller.style.ClearMarginPadding();
            scroller.highButton.style.display = DisplayStyle.None;
            scroller.lowButton.style.display = DisplayStyle.None;
            scroller.contentContainer.style.backgroundColor = RSTheme.Current.BackgroundColor2;
            var dragContainer = scroller.Q("unity-tracker");
            dragContainer.style.backgroundColor = new Color(0, 0, 0, 0.1f);
            dragContainer.style.SetRS_Style(RSBorder.Clear);
            scroller.Q("unity-dragger").style.backgroundColor = RSTheme.Current.BackgroundColor3;
            ApplyDirection(scroller);
        }
        public static void ApplyDirection(Scroller scroller)
        {
            var scrollerWidth = RSTheme.Current.LineHeight / 3f;
            var drag = scroller.Q("unity-dragger");
            foreach (var ve in scroller.slider.contentContainer.Children())
            {
                if (scroller.direction == SliderDirection.Horizontal)
                {
                    ve.style.height = scrollerWidth;
                    ve.style.width = StyleKeyword.Auto;
                }
                else
                {
                    ve.style.height = StyleKeyword.Auto;
                    ve.style.width = scrollerWidth;
                }
                ve.style.backgroundColor = Color.clear;
                ve.style.ClearMarginPadding();
            }
            if (scroller.direction == SliderDirection.Horizontal)
            {
                scroller.slider.style.height = scrollerWidth;
                scroller.style.height = scrollerWidth;
                scroller.slider.style.width = StyleKeyword.Auto;
                scroller.style.width = StyleKeyword.Auto;

                drag.style.height = Length.Percent(80);
                drag.style.width = Length.Percent(25);
                drag.style.top = Length.Percent(10);
                drag.style.left = StyleKeyword.Auto;
            }
            else
            {
                scroller.slider.style.width = scrollerWidth;
                scroller.style.width = scrollerWidth;
                scroller.slider.style.height = StyleKeyword.Auto;
                scroller.style.height = StyleKeyword.Auto;

                drag.style.height = Length.Percent(25);
                drag.style.width = Length.Percent(80);
                drag.style.top = StyleKeyword.Auto;
                drag.style.left = Length.Percent(10);
            }
        }
    }
}