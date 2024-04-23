using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class StringChoicesAttribute : Attribute
    {
        public List<string> choices = new List<string>();
    }

    [CustomRuntimeDrawer(typeof(string), RequiredAttribute = typeof(StringChoicesAttribute))]
    public class StringDropdownDrawer : RuntimeDrawer<string>
    {
        //public bool AllowNotExistValue = false;
        private List<string> choices;
        private bool isDirty = true;
        private RSTextElement field;
        private PopupElement popup;
        public void SetChoices(IEnumerable<string> choices)
        {
            this.choices ??= new();
            this.choices.Clear();
            this.choices.AddRange(choices);
            isDirty = true;
        }
        public void AddChoice(string choice)
        {
            choices.Add(choice);
            isDirty = true;
        }
        public void RemoveChoice(string choice)
        {
            choices.Remove(choice);
            isDirty = true;
        }
        public override void RepaintDrawer()
        {
            ((INotifyValueChanged<string>)field).SetValueWithoutNotify(value);
        }

        protected override void CreateGUI()
        {
            field = new RSTextElement("-");
            RSTheme.Current.ApplyFieldStyle(field);
            field.focusable = false;
            field.style.flexGrow = 1;
            field.style.unityTextAlign = TextAnchor.MiddleLeft;
            var icon = RSTheme.Current.CreateIconElement(RSTheme.Current.Icon.arrow, 90);
            icon.style.right = 0;
            icon.style.left = StyleKeyword.Auto;
            icon.style.paddingLeft = StyleKeyword.Auto;
            icon.style.marginLeft = StyleKeyword.Auto;
            field.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (isDirty)
                {
                    isDirty = false;
                    choices ??= new();
                    popup = RSContextMenu.CreatePopupMenu(choices, (val) => value = val);
                }
                popup.OpenBelow(field);
            });
            Add(field);
            field.Add(icon);
        }
        public override void ReciveAttribute(Attribute attribute)
        {
            StringChoicesAttribute attr = (StringChoicesAttribute)attribute;
            SetChoices(attr.choices);
        }
    }
}
