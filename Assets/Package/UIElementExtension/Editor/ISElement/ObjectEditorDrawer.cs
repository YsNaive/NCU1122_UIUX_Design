using NaiveAPI.UITK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NaiveAPI_Editor.UITK
{
    public class ObjectEditorDrawer : RuntimeDrawer<Object>
    {
        ObjectField field;
        public Type objectType
        {
            get => field.objectType; 
            set => field.objectType = value;
        }
        protected override void CreateGUI()
        {
            field = new();
            field.style.ClearMarginPadding();
            field[0].style.ClearMarginPadding();
            field[0].style.minHeight = RSTheme.Current.LineHeight;
            RSTheme.Current.ApplyFieldStyle(field[0]);
            foreach(var ve in field.ChildrenRecursive())
                RSTheme.Current.ApplyTextStyle(ve);
            field.RegisterValueChangedCallback(evt =>
            {
                evt.StopImmediatePropagation();
                value = evt.newValue;
            });
            Add(field);
        }
        public override void RepaintDrawer()
        {
            field.SetValueWithoutNotify(value);
        }
    }
}
