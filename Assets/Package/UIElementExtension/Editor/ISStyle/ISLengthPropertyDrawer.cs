using NaiveAPI.UITK;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSLength))]
    public class RSLengthPropertyDrawer : PropertyDrawer
    {
        const string choice_Pixel   = "Px";
        const string choice_Percent = "%";
        const string choice_Auto    = "Auto";
        const string choice_Initial = "Initial";
        const string choice_None    = "None";

        static string[] choices = new string[] { choice_Pixel, choice_Percent, choice_Auto, choice_Initial, choice_None };
        static string[] choices_short = new string[] { "Px", "%", "-", "-", " " };

        public static RSLength ParseFromSerializedProperty(SerializedProperty property)
        {
            RSLength result = new RSLength();
            result.value = property.FindPropertyRelative("value").floatValue;
            result.SetModeFlag((RSLength.ModeFlag)property.FindPropertyRelative("mode").enumValueFlag);
            return result;
        }
        public static void ApplyOnSerializedProperty(RSLength value,SerializedProperty property)
        {
            property.FindPropertyRelative("value").floatValue = value.value;
            property.FindPropertyRelative("mode").enumValueFlag = (int)value.Mode;
        }

        string getChoiceStr(RSLength length)
        {
            var key = length.keyword;
            if (key == StyleKeyword.Auto)    return choice_Auto;
            if (key == StyleKeyword.Initial) return choice_Initial;
            if (key == StyleKeyword.None)    return choice_None;
            if (length.unit == LengthUnit.Pixel)
                return choice_Pixel;
            else
                return choice_Percent;
        }
        void setChoiceStr(ref RSLength length, string choice)
        {
            if(choice == choice_Auto)
            {
                length.keyword = StyleKeyword.Auto;
                return;
            }
            if (choice == choice_Initial)
            {
                length.keyword = StyleKeyword.Initial;
                return;
            }
            if (choice == choice_None)
            {
                length.keyword = StyleKeyword.None;
                return;
            }
            if (choice == choice_Pixel)
            {
                length.keyword = StyleKeyword.Undefined;
                length.unit = LengthUnit.Pixel;
                return;
            }
            if(choice == choice_Percent)
            {
                length.keyword = StyleKeyword.Undefined;
                length.unit = LengthUnit.Percent;
                return;
            }
        }

        string[] m_choices, m_choices_short;
        public RSLength OnGUI(Rect position, RSLength value, GUIContent label)
        {
            if (m_choices == null) calculateChoice(value);

            int modeWidth = 36;
            Rect valueRect = position; valueRect.xMax -= (modeWidth + 4);
            Rect modeRect = position; modeRect.xMin = valueRect.xMax + 4;

            var currentChoice = getChoiceStr(value);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var tempColor = GUI.color;
            GUI.color = Color.clear;
            var currentIndex = Array.IndexOf(m_choices, currentChoice);
            currentIndex = EditorGUI.Popup(modeRect, currentIndex, m_choices);
            if (currentIndex < 0) currentIndex = 0;
            if (currentIndex > m_choices.Length) currentIndex = m_choices.Length-1;
            GUI.color = tempColor;
            EditorGUI.indentLevel = indent;
            GUI.Button(modeRect, m_choices_short[currentIndex]);

            setChoiceStr(ref value, m_choices[currentIndex]);

            if (value.keyword == StyleKeyword.Undefined)
            {
                value.value = EditorGUI.FloatField(valueRect,label , value.value);
            }
            else
            {
                var typeIn = EditorGUI.TextField(valueRect, label, value.keyword switch
                {
                    StyleKeyword.Auto => choice_Auto,
                    StyleKeyword.Initial => choice_Initial,
                    _ => choice_None
                });
                if(int.TryParse(typeIn.AsSpan(typeIn.Length-1), out var newVal))
                {
                    value.value = newVal;
                    setChoiceStr(ref value, choice_Pixel);
                }
            }

            return value;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var lengthObj = ParseFromSerializedProperty(property);
            if (m_choices == null) calculateChoice(lengthObj);
            lengthObj = OnGUI(position, lengthObj, label);
            ApplyOnSerializedProperty(lengthObj, property);
        }

        void calculateChoice(RSLength lengthObj)
        {
            List<string> tempChoice = new List<string>();
            List<string> tempChoiceShort = new List<string>();
            if ((lengthObj.Mode & RSLength.ModeFlag.CanBePixel) == RSLength.ModeFlag.CanBePixel)
            {
                tempChoice.Add(choices[0]);
                tempChoiceShort.Add(choices_short[0]);
            }
            if ((lengthObj.Mode & RSLength.ModeFlag.CanBePercent) == RSLength.ModeFlag.CanBePercent)
            {
                tempChoice.Add(choices[1]);
                tempChoiceShort.Add(choices_short[1]);
            }
            if ((lengthObj.Mode & RSLength.ModeFlag.CanBeAuto) == RSLength.ModeFlag.CanBeAuto)
            {
                tempChoice.Add(choices[2]);
                tempChoiceShort.Add(choices_short[2]);
            }
            if ((lengthObj.Mode & RSLength.ModeFlag.CanBeInitial) == RSLength.ModeFlag.CanBeInitial)
            {
                tempChoice.Add(choices[3]);
                tempChoiceShort.Add(choices_short[3]);
            }
            if ((lengthObj.Mode & RSLength.ModeFlag.CanBeNone) == RSLength.ModeFlag.CanBeNone)
            {
                tempChoice.Add(choices[4]);
                tempChoiceShort.Add(choices_short[4]);
            }
            m_choices = tempChoice.ToArray();
            m_choices_short = tempChoiceShort.ToArray();
        }
    }
}
