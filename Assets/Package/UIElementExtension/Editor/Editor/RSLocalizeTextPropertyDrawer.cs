using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSLocalizeText))]
    public class RSLocalizeTextPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var bindProp = property.FindPropertyRelative("isBind");
            var keyProp = property.FindPropertyRelative("m_key");
            var valueProp = property.FindPropertyRelative("m_value");
            Rect fieldRect = position;
            fieldRect.xMax -= 20;
            EditorGUI.BeginChangeCheck();
            Rect earthIconRect = position;
            earthIconRect.xMin = earthIconRect.xMax - earthIconRect.height;
            if(GUI.Button(earthIconRect, "", GUIStyle.none))
            {
                bindProp.boolValue = !bindProp.boolValue;
            }
            var icon = bindProp.boolValue? RSTheme.Current.Icon.earth : RSTheme.Current.Icon.disableEarth;
            UIElementExtensionEditorGUI.DrawSprite(earthIconRect, icon);
            if (bindProp.boolValue)
            {
                Rect pickBtnRect = fieldRect;
                pickBtnRect.xMin = fieldRect.xMax - 60;
                fieldRect.xMax -= 60;
                if(GUI.Button(pickBtnRect, "Pick"))
                {
                    RSLocalizationKeyPickerWindow.OpenTextKeyPicker(evt =>
                    {
                        keyProp.stringValue = evt;
                        keyProp.serializedObject.ApplyModifiedProperties();
                    });
                }
            }
            EditorGUI.PropertyField(fieldRect, bindProp.boolValue ? keyProp : valueProp, label);
        }
    }
}
