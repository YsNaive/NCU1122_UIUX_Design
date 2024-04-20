using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSPosition))]
    public class RSPositionPropertyDrawer : RSStyleComponentPropertyDrawer<RSPosition>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.position = (Position)prop.enumValueIndex;
            prop.Next(false);
            value.left = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.top = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.right = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.bottom = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            prop.enumValueIndex = (int)value.position;
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.left, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.top, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.right, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.bottom, prop);
        }

        RSLengthPropertyDrawer[] lenDrawers = new RSLengthPropertyDrawer[]
        { new(),new(),new(),new() };

        public override int GetRenderHeight()
        {
            return 20 * 5 + 5;
        }

        public override void OnGUI(Rect position)
        {
            Rect lineRect = position; lineRect.height = 18; lineRect.y++;

            BeginSetUnsetFieldByIndex(lineRect, 0);
            value.position = (Position)EditorGUI.EnumPopup(lineRect, "Position", value.position);
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 1);
            value.left = lenDrawers[0].OnGUI(lineRect, value.left, new GUIContent("Left"));
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 2);
            value.top = lenDrawers[1].OnGUI(lineRect, value.top, new GUIContent("Top"));
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 3);
            value.right = lenDrawers[2].OnGUI(lineRect, value.right, new GUIContent("Right"));
            EndSetUnsetField(); lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 4);
            value.bottom = lenDrawers[3].OnGUI(lineRect, value.bottom, new GUIContent("Bottom"));
            EndSetUnsetField();
        }
    }
}
