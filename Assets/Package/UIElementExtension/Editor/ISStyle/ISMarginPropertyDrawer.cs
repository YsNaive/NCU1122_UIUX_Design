using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSMargin))]
    public class RSMarginPropertyDrawer : RSStyleComponentPropertyDrawer<RSMargin>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
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
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.left, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.top, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.right, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.bottom, prop);
        }

        RSLengthPropertyDrawer[] lenDrawers = new RSLengthPropertyDrawer[]
        { new(),new(),new(),new(),new() };

        public override int GetRenderHeight()
        {
            return 20 * 5 + 5;
        }

        public override void OnGUI(Rect position)
        {
            position.height = 18; position.y++;

            BeginSetUnsetField(position, 15);
            var orgWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 45;
            var indentPos = position; indentPos.xMin += orgWidth;
            var newVal = lenDrawers[4].OnGUI(indentPos, value.any, new GUIContent("  Any"));
            if (EndSetUnsetField()) { value.any = newVal; }
            position.y += 20;
            EditorGUIUtility.labelWidth = orgWidth;

            BeginSetUnsetFieldByIndex(position, 0);
            value.left = lenDrawers[0].OnGUI(position, value.left, new GUIContent("Left"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 1);
            value.top = lenDrawers[1].OnGUI(position, value.top, new GUIContent("Top"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 2);
            value.right = lenDrawers[2].OnGUI(position, value.right, new GUIContent("Right"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 3);
            value.bottom = lenDrawers[3].OnGUI(position, value.bottom, new GUIContent("Bottom"));
            EndSetUnsetField();
        }
    }
}
