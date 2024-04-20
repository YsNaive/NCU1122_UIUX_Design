using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSFlex))]
    public class RSFlexPropertyDrawer : RSStyleComponentPropertyDrawer<RSFlex>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.basis = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.shrink = prop.floatValue;
            prop.Next(false);
            value.grow = prop.floatValue;
            prop.Next(false);
            value.direction = (FlexDirection)prop.enumValueIndex;
            prop.Next(false);
            value.wrap = (Wrap)prop.enumValueIndex;
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.basis, prop);
            prop.Next(false);
            prop.floatValue = value.shrink;
            prop.Next(false);
            prop.floatValue = value.grow;
            prop.Next(false);
            prop.enumValueIndex = (int)value.direction;
            prop.Next(false);
            prop.enumValueIndex = (int)value.wrap;
        }

        RSLengthPropertyDrawer lenDrawer = new();

        public override int GetRenderHeight()
        {
            return 20 * 4 + 5;
        }

        public override void OnGUI(Rect position)
        {
            Rect lineRect = position;
            lineRect.height = 18; lineRect.y++;

            BeginSetUnsetFieldByIndex(lineRect, 0);
            value.basis = lenDrawer.OnGUI(lineRect, value.basis, new GUIContent("Basis"));
            EndSetUnsetField();

            lineRect.y += 20;
            var orgLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 50;
            Rect fieldRect = lineRect; fieldRect.xMin += orgLabelWidth;
            Rect shrinkRect = fieldRect; shrinkRect.xMax -= (shrinkRect.width / 2f + 5);
            Rect growRect = fieldRect; growRect.xMin += (growRect.width / 2f + 5);
            BeginSetUnsetFieldByIndex(shrinkRect, 1);
            value.shrink = EditorGUI.FloatField(shrinkRect, new GUIContent("Shrink"), value.shrink);
            EndSetUnsetField();
            BeginSetUnsetFieldByIndex(growRect, 2);
            value.grow = EditorGUI.FloatField(growRect, new GUIContent("Grow"), value.grow);
            EndSetUnsetField();
            EditorGUIUtility.labelWidth = orgLabelWidth;

            lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 3);
            value.direction = (FlexDirection)EditorGUI.EnumPopup(lineRect, new GUIContent("FlexDirection"), value.direction);
            EndSetUnsetField();

            lineRect.y += 20;
            BeginSetUnsetFieldByIndex(lineRect, 4);
            value.wrap = (Wrap)EditorGUI.EnumPopup(lineRect, new GUIContent("Wrap"), value.wrap);
            EndSetUnsetField();
        }
    }
}
