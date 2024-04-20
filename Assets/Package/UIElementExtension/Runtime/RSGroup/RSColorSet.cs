using UnityEngine;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public class RSColorSet
    {
        [SerializeField]
        private Color[] colors = new Color[7];
        public Color this[int i]
        {
            get => colors[i];
            set => colors[i] = value;
        }
        public Color TextColor
        {
            get => colors[0];
            set => colors[0] = value;
        }
        public Color BackgroundColor
        {
            get => colors[1];
            set => colors[1] = value;
        }
        public Color BackgroundColor2
        {
            get => colors[2];
            set => colors[2] = value;
        }
        public Color BackgroundColor3
        {
            get => colors[3];
            set => colors[3] = value;
        }
        public Color FrontgroundColor
        {
            get => colors[4];
            set => colors[4] = value;
        }
        public Color FrontgroundColor2
        {
            get => colors[5];
            set => colors[5] = value;
        }
        public Color FrontgroundColor3
        {
            get => colors[6];
            set => colors[6] = value;
        }

        public void Generate(Color backgroundColor, Color frontgroundColor, float split = 9f)
        {
            var step = 1f / split;
            var bgRate = 1f;
            BackgroundColor   = backgroundColor * bgRate + frontgroundColor * (1f - bgRate); bgRate -= step;
            BackgroundColor2  = backgroundColor * bgRate + frontgroundColor * (1f - bgRate); bgRate -= step;
            BackgroundColor3  = backgroundColor * bgRate + frontgroundColor * (1f - bgRate); bgRate = step * 2;

            FrontgroundColor  = backgroundColor * bgRate + frontgroundColor * (1f - bgRate); bgRate -= step;
            FrontgroundColor2 = backgroundColor * bgRate + frontgroundColor * (1f - bgRate); bgRate -= step;
            FrontgroundColor3 = backgroundColor * bgRate + frontgroundColor * (1f - bgRate);

            //for(int i=0;i<colors.Length; i++)
            //{
            //    var hsv = colors[i].ToHSV(); hsv.v = Mathf.Pow(hsv.v, 1.55f);
            //    colors[i].FromHSV(hsv);
            //}
            //BackgroundColor   = _getColorFromBrightness(mainColor, backgroundBrightness);
            //BackgroundColor2  = _getColorFromBrightness(mainColor, backgroundBrightness - distance);
            //BackgroundColor3  = _getColorFromBrightness(mainColor, backgroundBrightness - distance * 2);
            //FrontgroundColor  = _getColorFromBrightness(mainColor, frontgroundBrightness);
            //FrontgroundColor2 = _getColorFromBrightness(mainColor, frontgroundBrightness + distance);
            //FrontgroundColor3 = _getColorFromBrightness(mainColor, frontgroundBrightness + distance * 2);
            TextColor = FrontgroundColor2;
        }
        private Color _getColorFromBrightness(Color color,float brightness)
        {
            return color.NewV(Mathf.Pow(brightness, 1.55f));
        }

        public RSColorSet DeepCopy()
        {
            var ret = new RSColorSet();
            for(int i = 0; i < 7; i++)
                ret[i] = this[i];
            return ret;
        }
    }
}
