using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSDisplay))]
    public class RSDisplayPropertyDrawer : RSStyleComponentPropertyDrawer<RSDisplay>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.opacity = prop.floatValue;
            prop.Next(false);
            value.display = (DisplayStyle)prop.enumValueIndex;
            prop.Next(false);
            value.visibility = (Visibility)prop.enumValueIndex;
            prop.Next(false);
            value.overflow = (Overflow)prop.enumValueIndex;
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            prop.floatValue = value.opacity;
            prop.Next(false);
            prop.enumValueIndex = (int)value.display;
            prop.Next(false);
            prop.enumValueIndex = (int)value.visibility;
            prop.Next(false);
            prop.enumValueIndex = (int)value.overflow;
        }

        public override int GetRenderHeight()
        {
            return 20 * 4 + 5;
        }

        public override void OnGUI(Rect position)
        {
            Rect line1Rect = position; line1Rect.height = 18; line1Rect.y++;
            Rect line2Rect = line1Rect; line2Rect.y += 20;
            Rect line3Rect = line2Rect; line3Rect.y += 20;
            Rect line4Rect = line3Rect; line4Rect.y += 20;

            BeginSetUnsetFieldByIndex(line1Rect, 0);
            EditorGUI.BeginChangeCheck();
            value.opacity = EditorGUI.Slider(line1Rect, "Opacity", value.opacity, 0f, 1f);
            EndSetUnsetField();

            BeginSetUnsetFieldByIndex(line2Rect, 1);
            value.display = (DisplayStyle)EditorGUI.EnumPopup(line2Rect, "Display", value.display);
            EndSetUnsetField();

            BeginSetUnsetFieldByIndex(line3Rect, 2);
            value.visibility = (Visibility)EditorGUI.EnumPopup(line3Rect, "Visibility", value.visibility);
            EndSetUnsetField();

            BeginSetUnsetFieldByIndex(line4Rect, 3);
            value.overflow = (Overflow)EditorGUI.EnumPopup(line4Rect, "Overflow", value.overflow);
            EndSetUnsetField();
        }

    }
}
