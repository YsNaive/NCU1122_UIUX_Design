using NaiveAPI.DocumentBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    [CustomRuntimeDrawer(typeof(Enum), DrawDerivedType = true)]
    public class EnumDrawer : RuntimeDrawer<Enum>
    {
        public override bool DynamicLayout => true;
        DSDropdown dropdown;
        public EnumDrawer()
        {
            dropdown = new DSDropdown();
            dropdown.RegisterCallback<PointerDownEvent>(evt => {
                if (evt.target == this)
                    evt.StopPropagation();
            });
            dropdown.SetValueWithoutNotify("NULL");
            dropdown.choices = new() { "NULL" };
            dropdown.RegisterValueChangedCallback(evt =>
            {
                object result;
                if (Enum.TryParse(value.GetType(), evt.newValue, out result))
                    SetValueWithoutUpdate((Enum)result);
                else
                    dropdown.SetValueWithoutNotify("NULL");
                evt.StopPropagation();
            });
            Add(dropdown);
        }
        protected override void OnCreateGUI()
        {
            dropdown.choices = new List<string>(value.GetType().GetEnumNames());
            dropdown.SetValueWithoutNotify(value.ToString());
        }

        public override void UpdateField()
        {
            dropdown.SetValueWithoutNotify(value.ToString());
        }
    }
}