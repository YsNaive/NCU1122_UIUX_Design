using NaiveAPI.DocumentBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    [CustomRuntimeDrawer(typeof(string), Priority = 10)]
    public class StringDrawer : RuntimeDrawer<string>
    {
        DSTextField field;
        protected override void OnCreateGUI()
        {
            field = new DSTextField();
            field.multiline = true;
            field[0].style.unityTextAlign = TextAnchor.MiddleLeft;
            field.RegisterValueChangedCallback(evt =>
            {
                SetValueWithoutUpdate(evt.newValue);
                if (evt.newValue.Contains('\n'))
                    LayoutExpand();
                else
                    LayoutInline();
                evt.StopPropagation();
            });
            Add(field);
        }
        public override void UpdateField()
        {
            field.SetValueWithoutNotify(value);
        }
    }
}