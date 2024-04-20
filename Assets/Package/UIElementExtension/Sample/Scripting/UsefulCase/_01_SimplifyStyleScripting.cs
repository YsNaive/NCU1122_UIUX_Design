using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK.Sample
{
    public class _01_SimplifyStyleScripting
    {
        // ---------------------------------- //
        //       origin method in unity       //
        // ---------------------------------- //
        public StyleLength paddingLeft = 0;
        public StyleLength paddingTop = 10;
        public StyleLength paddingRight = 20;
        public StyleLength paddingBottom = 30;
        public void OriginMethod(VisualElement ve)
        {
            ve.style.paddingLeft = paddingLeft;
            ve.style.paddingTop = paddingTop;
            ve.style.paddingRight = paddingRight;
            ve.style.paddingBottom = paddingBottom;
        }

        // ---------------------------------- //
        //           RSStyle method           //
        // ---------------------------------- //
        public RSPadding padding = new RSPadding
        {
            left = 0,
            top = 10,
            right = 20,
            bottom = 30,
        };
        public void RSStyleMethod(VisualElement ve)
        {
            padding.ApplyOn(ve);
        }

        // with not just padding
        RSStyle style = new RSStyle
        {
            Padding = new RSPadding
            {
                left = 0,
                top = 10,
                right = 20,
                bottom = 30,
            },
            Display = new RSDisplay
            {
                opacity = 0.5f,
            }
        };
    }
}
