using NaiveAPI.RuntimeWindowUtils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.DocumentBuilder
{
    [CustomRuntimeDrawer(typeof(bool), Priority = 10)]
    public class BooleanDrawer : RuntimeDrawer<bool>
    {
        DSToggle toggle;
        public BooleanDrawer()
        {
            toggle = new DSToggle();
            toggle.RegisterValueChangedCallback(evt =>
            {
                SetValueWithoutUpdate(evt.newValue);
                evt.StopPropagation();
            });
            Add(toggle);
        }
        protected override void OnCreateGUI() { }
        public override void UpdateField()
        {
            toggle.SetValueWithoutNotify(value);
        }
    }
}