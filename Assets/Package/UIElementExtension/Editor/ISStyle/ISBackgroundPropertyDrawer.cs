using NaiveAPI.UITK;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI_Editor.UITK
{
    [CustomPropertyDrawer(typeof(RSBackground))]
    public class RSBackgroundPropertyDrawer : RSStyleComponentPropertyDrawer<RSBackground>
    {
        readonly string[] backgroundChoices = new string[] { "Texture", "Sprite", "RenderTexture", "VectorImage" };

        public override void DecodeValueFromProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            value.SetUnsetFlag = prop.intValue;
            prop.Next(false);
            value.color = prop.colorValue;
            prop.Next(false);
            value.tintColor = prop.colorValue;
            prop.Next(false);
            value.sliceLeft = prop.intValue;
            prop.Next(false);
            value.sliceTop = prop.intValue;
            prop.Next(false);
            value.sliceRight = prop.intValue;
            prop.Next(false);
            value.sliceBottom = prop.intValue;
            prop.Next(false);
            value.background = RSBackground.ObjectToBackground(prop.objectReferenceValue);
        }

        public override void EncodeValueToProperty(SerializedProperty property)
        {
            var prop = property.Copy();
            prop.Next(true);
            prop.intValue = value.SetUnsetFlag;
            prop.Next(false);
            prop.colorValue = value.color;
            prop.Next(false);
            prop.colorValue = value.tintColor;
            prop.Next(false);
            prop.intValue = value.sliceLeft;
            prop.Next(false);
            prop.intValue = value.sliceTop;
            prop.Next(false);
            prop.intValue = value.sliceRight;
            prop.Next(false);
            prop.intValue = value.sliceBottom;
            prop.Next(false);
            prop.objectReferenceValue = RSBackground.BackgroundToObject(value.background);
        }
        int getBackgroundIndex(Background background)
        {
            if (background.texture != null)
                return 0;
            if (background.sprite != null)
                return 1;
            if (background.renderTexture != null)
                return 2;
            if (background.vectorImage != null)
                return 3;
            return 0;
        }
        int imgTypeIndex = -1;
        bool showSlice = false;

        public override int GetRenderHeight()
        {
            return (showSlice ? 20 * 4 : 0) + (20 * 5 + 5);
        }
        public override void OnGUI(Rect position)
        {
            Rect lineRect = position; lineRect.height = 18; lineRect.y++;

            BeginSetUnsetFieldByIndex(lineRect, 0);
            value.color = EditorGUI.ColorField(lineRect, "Color", value.color);
            EndSetUnsetField(); lineRect.y += 20;

            BeginSetUnsetFieldByIndex(lineRect, 1);
            Rect fieldRect = lineRect; fieldRect.xMax -= 80;
            Rect popupRect = lineRect; popupRect.xMin = popupRect.xMax - 75;
            if (imgTypeIndex == -1)
                imgTypeIndex = getBackgroundIndex(value.background);
            var newIndex = EditorGUI.Popup(popupRect, imgTypeIndex, backgroundChoices);
            if (imgTypeIndex != newIndex)
                value.background = new Background();
            imgTypeIndex = newIndex;
            var background = value.background;
            switch (imgTypeIndex)
            {
                case 0:
                    background.texture = (Texture2D)EditorGUI.ObjectField(fieldRect, "Image", value.background.texture, typeof(Texture2D), false);
                    break;
                case 1:
                    background.sprite = (Sprite)EditorGUI.ObjectField(fieldRect, "Image", value.background.sprite, typeof(Sprite), false);
                    break;
                case 2:
                    background.renderTexture = (RenderTexture)EditorGUI.ObjectField(fieldRect, "Image", value.background.renderTexture, typeof(RenderTexture), false);
                    break;
                case 3:
                    background.vectorImage = (VectorImage)EditorGUI.ObjectField(fieldRect, "Image", value.background.vectorImage, typeof(VectorImage), false);
                    break;
            }
            value.background = background;
            EndSetUnsetField(); lineRect.y += 20;

            BeginSetUnsetFieldByIndex(lineRect, 2);
            value.tintColor = EditorGUI.ColorField(lineRect, "TintColor", value.tintColor);
            EndSetUnsetField(); lineRect.y += 20;

            Rect foldoutRect = lineRect; foldoutRect.xMax = foldoutRect.xMin + EditorGUIUtility.labelWidth; foldoutRect.xMin += 8;
            Rect sliceShowRect = lineRect; sliceShowRect.xMin = foldoutRect.xMax;
            float sliceShowStep = sliceShowRect.width / 4;
            sliceShowRect.width = sliceShowStep - 10; sliceShowRect.x += 10;
            var orgWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 12;
            BeginSetUnsetFieldByIndex(sliceShowRect, 4);
            value.sliceLeft = EditorGUI.IntField(sliceShowRect, "L", value.sliceLeft);
            EndSetUnsetField(); sliceShowRect.x += sliceShowStep;

            BeginSetUnsetFieldByIndex(sliceShowRect, 5);
            value.sliceTop = EditorGUI.IntField(sliceShowRect, "T", value.sliceTop);
            EndSetUnsetField(); sliceShowRect.x += sliceShowStep;

            BeginSetUnsetFieldByIndex(sliceShowRect, 6);
            value.sliceRight = EditorGUI.IntField(sliceShowRect, "R", value.sliceRight);
            EndSetUnsetField(); sliceShowRect.x += sliceShowStep;

            BeginSetUnsetFieldByIndex(sliceShowRect, 7);
            value.sliceBottom = EditorGUI.IntField(sliceShowRect, "B", value.sliceBottom);
            EndSetUnsetField();
            EditorGUIUtility.labelWidth = orgWidth;

            lineRect.y += 20;
            showSlice = EditorGUI.BeginFoldoutHeaderGroup(foldoutRect, showSlice, "Slice");
            if (showSlice)
            {
                lineRect.xMin += 18;
                BeginSetUnsetFieldByIndex(lineRect, 4);
                value.sliceLeft = EditorGUI.IntField(lineRect, "SliceLeft", value.sliceLeft);
                EndSetUnsetField(); lineRect.y += 20;

                BeginSetUnsetFieldByIndex(lineRect, 5);
                value.sliceTop = EditorGUI.IntField(lineRect, "SliceTop", value.sliceTop);
                EndSetUnsetField(); lineRect.y += 20;

                BeginSetUnsetFieldByIndex(lineRect, 6);
                value.sliceRight = EditorGUI.IntField(lineRect, "SliceRight", value.sliceRight);
                EndSetUnsetField(); lineRect.y += 20;

                BeginSetUnsetFieldByIndex(lineRect, 7);
                value.sliceBottom = EditorGUI.IntField(lineRect, "SliceBottom", value.sliceBottom);
                EndSetUnsetField();
            }
            EditorGUI.EndFoldoutHeaderGroup();
        }
    }
}
