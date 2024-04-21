using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public abstract class NumberRangeDrawer<T> : RuntimeDrawer<T>
    {
        public RSSlider slider;
        public RSTextField textfield;
        public NumberRangeDrawer()
        {
            var border = new RSBorder(Color.clear, 0);
            slider = new RSSlider();
            slider.style.marginRight = RSTheme.Current.VisualMargin;
            slider.RegisterValueChangedCallback(evt =>
            {
                SetValueWithoutRepaint(CaseNumber(evt.newValue));
                textfield.SetValueWithoutNotify(CaseNumber(evt.newValue).ToString());
                evt.StopPropagation();
            });
            textfield = new RSTextField();
            textfield.RegisterCallback<FocusOutEvent>(evt =>
            {
                float result;
                if (float.TryParse(textfield.value, out result))
                {
                    slider.value = Math.Clamp(result, slider.lowValue, slider.highValue);
                    textfield.SetValueWithoutNotify(CaseNumber(slider.value).ToString());
                    SetValueWithoutRepaint(CaseNumber(slider.value));
                }
                else
                    textfield.SetValueWithoutNotify(CaseNumber(result).ToString());
                evt.StopPropagation();
            });
            Add(new RSHorizontal(slider, null, null, null, textfield));
        }

        protected override void CreateGUI()
        {
        }
        public override void RepaintDrawer()
        {
            if (textfield.IsFocusedOnPanel()) return;
            textfield.value = (value.ToString());
            slider.value = float.Parse(value.ToString());
        }
        public override void ReciveAttribute(Attribute attribute)
        {
            var range = attribute as RangeAttribute;
            slider.lowValue = range.min;
            slider.highValue = range.max;
        }
        abstract protected bool TryParseNumber(string value, out T result);
        abstract protected T CaseNumber(float num);
    }
    [CustomRuntimeDrawer(typeof(int), Priority = 99, RequiredAttribute =typeof(RangeAttribute))]
    public class IntegerRangeDrawer : NumberRangeDrawer<int>
    {
        protected override int CaseNumber(float num)
        {
            return (int)num;
        }
        protected override bool TryParseNumber(string value, out int result)
        {
            return int.TryParse(value, out result);
        }
    }
    [CustomRuntimeDrawer(typeof(float), Priority = 99, RequiredAttribute = typeof(RangeAttribute))]
    public class FloatRangeDrawer : NumberRangeDrawer<float>
    {
        protected override float CaseNumber(float num)
        {
            return num;
        }
        protected override bool TryParseNumber(string value, out float result)
        {
            return float.TryParse(value, out result);
        }
    }
}