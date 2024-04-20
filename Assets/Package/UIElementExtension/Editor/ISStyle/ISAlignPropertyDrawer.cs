using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSAlign))]
    public class RSAlignPropertyDrawer : RSStyleComponentPropertyDrawer<RSAlign>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.alignSelf = (Align)prop.enumValueIndex;
            prop.Next(false);
            value.alignItems = (Align)prop.enumValueIndex;
            prop.Next(false);
            value.justifyContent = (Justify)prop.enumValueIndex;
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            prop.enumValueIndex = (int)value.alignSelf;
            prop.Next(false);
            prop.enumValueIndex = (int)value.alignItems;
            prop.Next(false);
            prop.enumValueIndex = (int)value.justifyContent;
        }

        public override int GetRenderHeight()
        {
            return 20 * 3 + 5;
        }

        public override void OnGUI(Rect position)
        {
            Rect lineRect = position; lineRect.height = 18; lineRect.y++;

            BeginSetUnsetFieldByIndex(lineRect, 0);
            value.alignSelf = (Align)EditorGUI.EnumPopup(lineRect, "AlignSelf", value.alignSelf);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 1);
            value.alignItems = (Align)EditorGUI.EnumPopup(lineRect, "AlignItems", value.alignItems);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 2);
            value.justifyContent = (Justify)EditorGUI.EnumPopup(lineRect, "JustifyContent", value.justifyContent);
            EndSetUnsetField(); lineRect.y += 20;
        }
    }
}
