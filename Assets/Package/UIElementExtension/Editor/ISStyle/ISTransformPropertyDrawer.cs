using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSTransform))]
    public class RSTransformPropertyDrawer : RSStyleComponentPropertyDrawer<RSTransform>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.pivotX = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.pivotY = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.x = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.y = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.scale = prop.vector2Value;
            prop.Next(false);
            value.rotateDeg = prop.floatValue;
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.pivotX, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.pivotY, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.x, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.y, prop);
            prop.Next(false);
            prop.vector2Value = value.scale;
            prop.Next(false);
            prop.floatValue = value.rotateDeg;
        }

        RSLengthPropertyDrawer[] lenDrawers = new RSLengthPropertyDrawer[] { new(), new(), new(), new() };

        public override int GetRenderHeight()
        {
            return 20 * 4 + 5;
        }

        public override void OnGUI(Rect position)
        {
            position.height = 18; position.y++;
            Rect labelRect = position; labelRect.xMax = labelRect.xMin + EditorGUIUtility.labelWidth;
            Rect fieldRect = position; fieldRect.xMin += EditorGUIUtility.labelWidth;
            Rect xRect = fieldRect; xRect.width /= 2;
            Rect yRect = xRect; yRect.x += yRect.width;
            xRect.xMax -= 10; xRect.xMin += 2;
            yRect.xMax -= 10;
            GUIContent xContent = new GUIContent("X");
            GUIContent yContent = new GUIContent("Y");

            EditorGUI.LabelField(labelRect, "Pivot"); labelRect.y += 20;
            EditorGUI.LabelField(labelRect, "Position");

            var originWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 13;
            BeginSetUnsetFieldByIndex(position, 0);
            value.pivotX = lenDrawers[0].OnGUI(xRect, value.pivotX, xContent);
            value.pivotY = lenDrawers[1].OnGUI(yRect, value.pivotY, yContent);
            EndSetUnsetField(); position.y += 20;
            fieldRect.y += 20; xRect.y += 20; yRect.y += 20;
            BeginSetUnsetFieldByIndex(position, 1);
            value.x = lenDrawers[0].OnGUI(xRect, value.x, xContent);
            value.y = lenDrawers[1].OnGUI(yRect, value.y, yContent);
            EndSetUnsetField(); position.y += 20;
            EditorGUIUtility.labelWidth = originWidth;

            BeginSetUnsetFieldByIndex(position, 2);
            value.scale = EditorGUI.Vector2Field(position, "Scale", value.scale);
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 3);
            value.rotateDeg = EditorGUI.FloatField(position, "RotateDeg", value.rotateDeg);
            EndSetUnsetField();
        }
    }
}
