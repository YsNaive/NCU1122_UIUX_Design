using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(string), Priority = 10)]
    public class StringDrawer : RuntimeDrawer<string>
    {
        public bool multiline
        {
            get  => field.multiline; 
            set => field.multiline = value;
        }
        RSTextField field;
        protected override void CreateGUI()
        {
            field = new RSTextField();
            multiline = false;
            field[0].style.unityTextAlign = TextAnchor.MiddleLeft;
            field.RegisterValueChangedCallback(evt =>
            {
                SetValueWithoutRepaint(evt.newValue);
                if (multiline)
                {
                    if (evt.newValue.Contains('\n'))
                        LayoutExpand();
                    else
                        LayoutInline();
                }
                evt.StopImmediatePropagation();
            });
            Add(field);
            SetValueWithoutNotify("");
        }
        public override void RepaintDrawer()
        {
            field.SetValueWithoutNotify(value);
        }
    }
}