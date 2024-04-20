using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSBorder))]
    public class RSBorderPropertyDrawer : RSStyleComponentPropertyDrawer<RSBorder>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.leftColor = prop.colorValue;
            prop.Next(false);
            value.topColor = prop.colorValue;
            prop.Next(false);
            value.rightColor = prop.colorValue;
            prop.Next(false);
            value.bottomColor = prop.colorValue;
            prop.Next(false);
            value.leftWidth = prop.floatValue;
            prop.Next(false);
            value.topWidth = prop.floatValue;
            prop.Next(false);
            value.rightWidth = prop.floatValue;
            prop.Next(false);
            value.bottomWidth = prop.floatValue;
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            prop.colorValue = value.leftColor;
            prop.Next(false);
            prop.colorValue = value.topColor;
            prop.Next(false);
            prop.colorValue = value.rightColor;
            prop.Next(false);
            prop.colorValue = value.bottomColor;
            prop.Next(false);
            prop.floatValue = value.leftWidth;
            prop.Next(false);
            prop.floatValue = value.topWidth;
            prop.Next(false);
            prop.floatValue = value.rightWidth;
            prop.Next(false);
            prop.floatValue = value.bottomWidth;
        }

        public override int GetRenderHeight()
        {
            return 20 * 10 + 5;
        }

        public override void OnGUI(Rect position)
        {
            Rect lineRect = position; lineRect.height = 18; lineRect.y++;

            BeginSetUnsetField(lineRect, RSBorder.F_AnyColor);
            var orgWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 75;
            var indentPos = lineRect; indentPos.xMin += orgWidth;
            var newColor = EditorGUI.ColorField(indentPos, new GUIContent("  Any Color"), value.anyColor);
            if (EndSetUnsetField()) { value.anyColor = newColor; }
            lineRect.y += 20;
            EditorGUIUtility.labelWidth = orgWidth;

            BeginSetUnsetFieldByIndex(lineRect, 0);
            value.leftColor = EditorGUI.ColorField(lineRect, "LeftColor", value.leftColor);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 1);
            value.topColor = EditorGUI.ColorField(lineRect, "TopColor", value.topColor);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 2);
            value.rightColor = EditorGUI.ColorField(lineRect, "RightColor", value.rightColor);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 3);
            value.bottomColor = EditorGUI.ColorField(lineRect, "BottomColor", value.bottomColor);
            EndSetUnsetField(); lineRect.y += 20;


            BeginSetUnsetField(lineRect, RSBorder.F_AnyWidth);
            orgWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 75;
            indentPos = lineRect; indentPos.xMin += orgWidth;
            var newWidth = EditorGUI.FloatField(indentPos, new GUIContent("  Any Width"), value.anyWidth);
            if (EndSetUnsetField()) { value.anyWidth = newWidth; }
            lineRect.y += 20;
            EditorGUIUtility.labelWidth = orgWidth;

            BeginSetUnsetFieldByIndex(lineRect, 4);
            value.leftWidth = EditorGUI.FloatField(lineRect, "LeftWidth", value.leftWidth);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 5);
            value.topWidth = EditorGUI.FloatField(lineRect, "TopWidth", value.topWidth);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 6);
            value.rightWidth = EditorGUI.FloatField(lineRect, "RightWidth", value.rightWidth);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 7);
            value.bottomWidth = EditorGUI.FloatField(lineRect, "BottomWidth", value.bottomWidth);
            EndSetUnsetField(); lineRect.y += 20;
        }
    }
}
