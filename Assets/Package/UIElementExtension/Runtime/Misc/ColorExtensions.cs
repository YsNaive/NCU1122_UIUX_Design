using UnityEngine;

namespace NaiveAPI.UITK
{
    public static class ColorExtensions
    {
        public static (float h, float s, float v) ToHSV(this Color color)
        {
            Color.RGBToHSV(color, out var h, out var s, out var v);
            return (h, s, v);
        }
        public static void FromHSV(this ref Color color, (float h, float s, float v) hsv)
        {
            color = Color.HSVToRGB(hsv.h, hsv.s, hsv.v);
        }
        public static int ToU32(this Color color)
        {
            int result = 0;
            result |= (int)Mathf.Clamp(color.r * 255, 0, 255);
            result <<= 8;
            result |= (int)Mathf.Clamp(color.g * 255, 0, 255);
            result <<= 8;
            result |= (int)Mathf.Clamp(color.b * 255, 0, 255);
            result <<= 8;
            result |= (int)Mathf.Clamp(color.a * 255, 0, 255);
            return result;
        }
        public static void FromU32(this ref Color color, int u32)
        {
            color.a = (u32 & 255) / 255f;
            u32 >>= 8;
            color.b = (u32 & 255) / 255f;
            u32 >>= 8;
            color.g = (u32 & 255) / 255f;
            u32 >>= 8;
            color.r = (u32 & 255) / 255f;
        }
        /// <summary>
        /// h of hsv
        /// </summary>
        public static float GetHue(this Color color)
        {
            return color.ToHSV().h;
        }
        /// <summary>
        /// s of hsv
        /// </summary>
        public static float GetSaturation(this Color color)
        {
            return color.ToHSV().s;
        }
        /// <summary>
        /// v of hsv
        /// </summary>
        public static float GetBrightness(this Color color)
        {
            return color.ToHSV().v;
        }

        public static Color NewR(this Color color, float r)
        {
            color.r = r;
            return color;
        }
        public static Color NewG(this Color color, float g)
        {
            color.g = g;
            return color;
        }
        public static Color NewB(this Color color, float b)
        {
            color.b = b;
            return color;
        }
        public static Color NewA(this Color color, float a)
        {
            color.a = a;
            return color;
        }
        public static Color NewH(this Color color, float h)
        {
            var hsv = color.ToHSV();
            return Color.HSVToRGB(h, hsv.s, hsv.v);
        }
        public static Color NewS(this Color color, float s)
        {
            var hsv = color.ToHSV();
            return Color.HSVToRGB(hsv.h, s, hsv.v);
        }
        public static Color NewV(this Color color, float v)
        {
            var hsv = color.ToHSV();
            return Color.HSVToRGB(hsv.h, hsv.s, v);
        }
    }
}
