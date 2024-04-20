using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [CustomRuntimeDrawer(typeof(Enum), DrawDerivedType = true)]
    public class EnumDrawer : EnumDrawer<Enum> { }
    public class EnumDrawer<T> : RuntimeDrawer<T>
        where T : Enum
    {
        public override bool DynamicLayout => true;
        public Type EnumType
        {
            get => m_EnumType;
            set
            {
                if (m_EnumType != value)
                {
                    dropdown.SetChoices(new List<string>(value.GetEnumNames()));
                    m_EnumType = value;
                }
            }
        }
        private Type m_EnumType = null;
        StringDropdownDrawer dropdown;
        public EnumDrawer()
        {
            dropdown = new();
            dropdown.enableIcon = false;
            dropdown.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.target == this)
                    evt.StopPropagation();
            });
            dropdown.SetValueWithoutNotify("NULL");
            dropdown.SetChoices(new List<string> { "NULL" });
            dropdown.OnValueChanged += () =>
            {
                object result;
                if (Enum.TryParse(value.GetType(), dropdown.value, out result))
                    value = (T)result;
                else
                    dropdown.SetValueWithoutNotify("NULL");
            };
            Add(dropdown.contentContainer);
        }
        protected override void CreateGUI()
        {
            EnumType = value.GetType();
            dropdown.SetValueWithoutNotify(value.ToString());
        }

        public override void RepaintDrawer()
        {
            dropdown.SetValueWithoutNotify(value.ToString());
        }
    }
}