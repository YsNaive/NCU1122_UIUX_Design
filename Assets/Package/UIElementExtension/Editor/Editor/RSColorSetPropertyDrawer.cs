using NaiveAPI.UITK;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSColorSet))]
    public class RSColorSetPropertyDrawer : PropertyDrawer
    {
        bool isGenerate = false;
        RSColorSet generateTemp = new();
        Color generateBG = new Color(.25f, .25f, .25f);
        Color generateFG = new Color(.9f, .9f, .9f);
        float generateSplit = 9f;
        Color[] copy = new Color[7];
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var colorsProp = property.FindPropertyRelative("colors");
            position.height = 18;
            var foldoutPos = position;
            foldoutPos.xMin -= 16;
            if (property.isExpanded) foldoutPos.xMax -= 110;
            property.isExpanded = EditorGUI.BeginFoldoutHeaderGroup(foldoutPos, property.isExpanded, label);
            EditorGUI.EndFoldoutHeaderGroup();
            var colorIconPos = position;
            colorIconPos.x = colorIconPos.x + EditorGUIUtility.labelWidth;
            colorIconPos.y += 2;
            colorIconPos.height -= 4;
            colorIconPos.x += 4;
            colorIconPos.width = colorIconPos.height;
            for (int i = 0; i < 7; i++)
            {
                EditorGUI.DrawRect(colorIconPos, colorsProp.GetArrayElementAtIndex(i).colorValue);
                colorIconPos.x += colorIconPos.width + 2;
            }
            if (!property.isExpanded) return;
            var generateBtnPosition = position;
            generateBtnPosition.xMin = position.xMax - 110;
            if(!isGenerate)
            {
                if (GUI.Button(generateBtnPosition, "Generate"))
                {
                    isGenerate = true;
                    for (int i = 0; i < 7; i++)
                    {
                        copy[i] = colorsProp.GetArrayElementAtIndex(i).colorValue;
                    }
                    generateBG = copy[1];
                    generateFG = copy[6];
                }
            }
            else
            {
                var guiColor = GUI.color;
                var leftBtnPosition = generateBtnPosition;
                leftBtnPosition.width = leftBtnPosition.width / 2;
                var rightBtnPosition = leftBtnPosition;
                rightBtnPosition.x = leftBtnPosition.xMax;
                GUI.color = RSTheme.Current.SuccessColorSet.BackgroundColor.NewV(0.9f);
                if(GUI.Button(leftBtnPosition, "Apply")){
                    isGenerate = false;
                }
                GUI.color = RSTheme.Current.DangerColorSet.BackgroundColor.NewV(0.9f);
                if (GUI.Button(rightBtnPosition, "Cancel")){
                    isGenerate = false;
                    for (int i = 0; i < 7; i++)
                    {
                        colorsProp.GetArrayElementAtIndex(i).colorValue = copy[i];
                    }
                }
                GUI.color = guiColor;
            }
            if (isGenerate)
            {
                EditorGUI.BeginChangeCheck();
                position.y += position.height;
                generateSplit = EditorGUI.Slider(position, "Split", generateSplit, 5f,15f);
                position.y += position.height;
                generateBG = EditorGUI.ColorField(position, "Background", generateBG);
                position.y += position.height;
                generateFG = EditorGUI.ColorField(position, "Frontground", generateFG);

                if (EditorGUI.EndChangeCheck())
                {
                    generateTemp.Generate(generateBG, generateFG, generateSplit);
                    for (int i = 0; i < 7; i++)
                    {
                        colorsProp.GetArrayElementAtIndex(i).colorValue = generateTemp[i];
                    }
                }
                return;
            }
            EditorGUI.indentLevel++;
            for (int i = 0; i < 7; i++)
            {
                position.y += position.height;
                EditorGUI.PropertyField(position, colorsProp.GetArrayElementAtIndex(i));
            }
            EditorGUI.indentLevel--;
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var ret = 18f;
            if (isGenerate)
            {
                ret += 18 * 3;
                return ret;
            }
            if (property.isExpanded)
                ret += 18 * 7;
            return ret;
        }
    }
}
