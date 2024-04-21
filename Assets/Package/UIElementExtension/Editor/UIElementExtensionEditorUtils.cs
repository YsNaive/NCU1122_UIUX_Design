using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NaiveAPI_Editor.UITK
{
    public static class UIElementExtensionEditorUtils
    {
        public static void DrawSprite(Rect position, Sprite sprite)
        {
            Vector2 textureSize = new Vector2(sprite.texture.width, sprite.texture.height);
            Vector2 spriteSize = new Vector2(sprite.textureRect.width, sprite.textureRect.height);

            Rect coords = sprite.textureRect;
            coords.position /= textureSize;
            coords.size /= textureSize;

            Vector2 ratio = position.position / spriteSize;
            float minRatio = Mathf.Min(ratio.x, ratio.y);

            Vector2 center = position.center;
            position.position = spriteSize * minRatio;
            position.center = center;

            GUI.DrawTextureWithTexCoords(position, sprite.texture, coords);
        }
    }
}
