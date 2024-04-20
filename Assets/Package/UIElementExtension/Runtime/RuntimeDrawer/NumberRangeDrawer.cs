using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public abstract class NumberRangeDrawer<T> : RuntimeDrawer<T>
    {
        Slider slider;
        RSTextField field;
        public NumberRangeDrawer()
        {
            var border = new RSBorder(Color.clear, 0);
            slider = new Slider() { direction = SliderDirection.Horizontal };
            slider.style.ClearMarginPadding();
            slider.style.minHeight = RSTheme.Current.LineHeight;
            slider.style.paddingLeft = RSTheme.Current.LineHeight / 2;
            var tracker = slider.Q("unity-tracker");
            tracker.style.backgroundColor = RSTheme.Current.BackgroundColor2;
            tracker.style.SetRS_Style(border);
            var drager = slider.Q("unity-dragger");
            drager.style.SetRS_Style(RSRadius.Percent(100));
            drager.style.ClearMarginPadding();
            var size = RSTheme.Current.LineHeight;
            tracker.style.marginTop = 0;
            tracker.style.top = size * 0.4f;
            tracker.style.height = size * 0.2f;
            drager.style.width = size * 0.6f;
            drager.style.height = size * 0.6f;
            drager.style.backgroundColor = RSTheme.Current.FrontgroundColor;
            drager.style.top = size * 0.2f;
            drager.style.SetRS_Style(border);
            slider.RegisterValueChangedCallback(evt =>
            {
                SetValueWithoutRepaint(CaseNumber(evt.newValue));
                field.SetValueWithoutNotify(CaseNumber(evt.newValue).ToString());
                evt.StopPropagation();
            });
            field = new RSTextField();
            field.RegisterCallback<FocusOutEvent>(evt =>
            {
                float result;
                if (float.TryParse(field.value, out result))
                {
                    slider.value = Math.Clamp(result, slider.lowValue, slider.highValue);
                    field.SetValueWithoutNotify(CaseNumber(slider.value).ToString());
                    SetValueWithoutRepaint(CaseNumber(slider.value));
                }
                else
                    field.SetValueWithoutNotify(CaseNumber(result).ToString());
                evt.StopPropagation();
            });
            Add(new RSHorizontal(slider, null, null, null, field));
        }

        protected override void CreateGUI()
        {
        }
        public override void RepaintDrawer()
        {
            if (field.IsFocusedOnPanel()) return;
            field.value = (value.ToString());
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