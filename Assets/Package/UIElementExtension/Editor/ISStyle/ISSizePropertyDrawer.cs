using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSSize))]
    public class RSSizePropertyDrawer : RSStyleComponentPropertyDrawer<RSSize>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.width = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.height = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.minWidth = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.minHeight = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.maxWidth = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.maxHeight = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.width, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.height, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.minWidth, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.minHeight, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.maxWidth, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.maxHeight, prop);
        }

        RSLengthPropertyDrawer[] lenDrawers = new RSLengthPropertyDrawer[] { new(), new(), new(), new(), new(), new() };

        public override int GetRenderHeight()
        {
            return 20 * 3 + 5;
        }

        public override void OnGUI(Rect position)
        {
            Rect labelRect = position; labelRect.width = 40; labelRect.height = 18; labelRect.y++;
            Rect widthRect = labelRect; widthRect.width = (position.width - labelRect.width) / 2; widthRect.x += labelRect.width; widthRect.y++;
            Rect heightRect = widthRect; heightRect.x += widthRect.width;
            widthRect.xMax -= 6; heightRect.xMin += 4; heightRect.xMax -= 4;
            EditorGUI.LabelField(labelRect, "size"); labelRect.y += 20;
            EditorGUI.LabelField(labelRect, "max"); labelRect.y += 20;
            EditorGUI.LabelField(labelRect, "min");
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 18;
            var wContent = new GUIContent("w");
            var hContent = new GUIContent("h");

            BeginSetUnsetFieldByIndex(widthRect, 0);
            value.width = lenDrawers[0].OnGUI(widthRect, value.width, wContent);
            EndSetUnsetField(); widthRect.y += 20;
            BeginSetUnsetFieldByIndex(widthRect, 4);
            value.maxWidth = lenDrawers[1].OnGUI(widthRect, value.maxWidth, wContent);
            EndSetUnsetField(); widthRect.y += 20;
            BeginSetUnsetFieldByIndex(widthRect, 2);
            value.minWidth = lenDrawers[2].OnGUI(widthRect, value.minWidth, wContent);
            EndSetUnsetField();

            BeginSetUnsetFieldByIndex(heightRect, 1);
            value.height = lenDrawers[3].OnGUI(heightRect, value.height, hContent);
            EndSetUnsetField(); heightRect.y += 20;
            BeginSetUnsetFieldByIndex(heightRect, 5);
            value.maxHeight = lenDrawers[4].OnGUI(heightRect, value.maxHeight, hContent);
            EndSetUnsetField(); heightRect.y += 20;
            BeginSetUnsetFieldByIndex(heightRect, 3);
            value.minHeight = lenDrawers[5].OnGUI(heightRect, value.minHeight, hContent);
            EndSetUnsetField();
            EditorGUIUtility.labelWidth = labelWidth;
        }
    }
}
