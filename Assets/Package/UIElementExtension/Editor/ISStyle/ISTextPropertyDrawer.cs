using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSText))]
    public class RSTextPropertyDrawer : RSStyleComponentPropertyDrawer<RSText>
    {
        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.fontAsset = (UnityEngine.TextCore.Text.FontAsset)prop.objectReferenceValue;
            prop.Next(false);
            value.fontStyle = (FontStyle)prop.enumValueIndex;
            prop.Next(false);
            value.size = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.color = prop.colorValue;
            prop.Next(false);
            value.anchor = (TextAnchor)prop.enumValueIndex;
            prop.Next(false);
            value.wrap = (WhiteSpace)prop.enumValueIndex;
            prop.Next(false);
            value.letterSpacing = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.wordSpacing = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
            prop.Next(false);
            value.paragraphSpacing = RSLengthPropertyDrawer.ParseFromSerializedProperty(prop);
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            prop.objectReferenceValue = value.fontAsset;
            prop.Next(false);
            prop.enumValueIndex = (int)value.fontStyle;
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.size, prop);
            prop.Next(false);
            prop.colorValue = value.color;
            prop.Next(false);
            prop.enumValueIndex = (int)value.anchor;
            prop.Next(false);
            prop.enumValueIndex = (int)value.wrap;
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.letterSpacing, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.wordSpacing, prop);
            prop.Next(false);
            RSLengthPropertyDrawer.ApplyOnSerializedProperty(value.paragraphSpacing, prop);
        }
        RSLengthPropertyDrawer[] lenDrawers = new RSLengthPropertyDrawer[] { new(), new(), new(), new() };

        public override int GetRenderHeight()
        { return 20 * 9 + 5; }

        public override void OnGUI(Rect position)
        {
            position.height = 18; position.y++;
            BeginSetUnsetFieldByIndex(position, 0);
            value.fontAsset = (FontAsset)EditorGUI.ObjectField(position, "FontAsset", value.fontAsset, typeof(FontAsset), false);
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 1);
            value.fontStyle = (FontStyle)EditorGUI.EnumPopup(position, "FontStyle", value.fontStyle);
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 2);
            value.size = lenDrawers[0].OnGUI(position, value.size, new GUIContent("Size"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 3);
            value.color = EditorGUI.ColorField(position, "Color", value.color);
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 4);
            value.anchor = (TextAnchor)EditorGUI.EnumPopup(position, "Anchor", value.anchor);
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 5);
            value.wrap = (WhiteSpace)EditorGUI.EnumPopup(position, "Wrap", value.wrap);
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 6);
            value.letterSpacing = lenDrawers[1].OnGUI(position, value.letterSpacing, new GUIContent("LetterSpacing"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 7);
            value.wordSpacing = lenDrawers[2].OnGUI(position, value.wordSpacing, new GUIContent("WordSpacing"));
            EndSetUnsetField(); position.y += 20;
            BeginSetUnsetFieldByIndex(position, 8);
            value.paragraphSpacing = lenDrawers[3].OnGUI(position, value.paragraphSpacing, new GUIContent("ParagraphSpacing"));
            EndSetUnsetField(); position.y += 20;
        }
    }
}
