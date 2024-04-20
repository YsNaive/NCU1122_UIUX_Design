using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSBackground : RSStyleComponent<RSBackground>, ISerializationCallbackReceiver
    {
        public const int F_Color       = 1 << 0;
        public const int F_Background  = 1 << 1;
        public const int F_TintColor   = 1 << 2;
        [System.Obsolete]
        public const int F_ScaleMode   = 1 << 3;
        public const int F_SliceLeft   = 1 << 4;
        public const int F_SliceTop    = 1 << 5;
        public const int F_SliceRight  = 1 << 6;
        public const int F_SliceBottom = 1 << 7;
        public const int F_Any = F_Color | F_Background | F_TintColor | F_SliceLeft | F_SliceTop | F_SliceRight | F_SliceBottom;

        [SerializeField] private Color      m_Color = Color.black;
        /*---s_img----*/ private Background m_Background;
        [SerializeField] private Color      m_TintColor = Color.white;
        [SerializeField] private int m_SliceLeft   = 0;
        [SerializeField] private int m_SliceTop    = 0;
        [SerializeField] private int m_SliceRight  = 0;
        [SerializeField] private int m_SliceBottom = 0;
        #region properties get set
        public Color color
        {
            get => m_Color;
            set
            {
                m_Color = value;
                m_flag |= F_Color;
            }
        }
        public Background background
        {
            get => m_Background;
            set
            {
                m_Background = value;
                m_flag |= F_Background;
            }
        }
        public Color tintColor
        {
            get => m_TintColor;
            set
            {
                m_TintColor = value;
                m_flag |= F_TintColor;
            }
        }
        public int sliceLeft
        {
            get => m_SliceLeft;
            set
            {
                m_SliceLeft = value;
                m_flag |= F_SliceLeft;
            }
        }
        public int sliceTop
        {
            get => m_SliceTop;
            set
            {
                m_SliceTop = value;
                m_flag |= F_SliceTop;
            }
        }
        public int sliceRight
        {
            get => m_SliceRight;
            set
            {
                m_SliceRight = value;
                m_flag |= F_SliceRight;
            }
        }
        public int sliceBottom
        {
            get => m_SliceBottom;
            set
            {
                m_SliceBottom = value;
                m_flag |= F_SliceBottom;
            }
        }
        #endregion

        [SerializeField]  private Object s_img;

        public override RSStyleFlag StyleFlag => RSStyleFlag.Background;
        public override int PropertyCount => 8;

        public override void SetValueToDefault(int flag)
        {
            if ((flag & F_Color)       == F_Color)       color       = Color.black;
            if ((flag & F_Background)  == F_Background)  background  = new Background();
            if ((flag & F_TintColor)   == F_TintColor)   tintColor   = Color.white;
            if ((flag & F_SliceLeft)   == F_SliceLeft)   sliceLeft   = 0;
            if ((flag & F_SliceTop)    == F_SliceTop)    sliceTop    = 0;
            if ((flag & F_SliceRight)  == F_SliceRight)  sliceRight  = 0;
            if ((flag & F_SliceBottom) == F_SliceBottom) sliceBottom = 0;
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_Color))       style.backgroundColor = m_Color;
            if (GetFlag(F_Background))  style.backgroundImage = m_Background;
            if (GetFlag(F_TintColor))   style.unityBackgroundImageTintColor = m_TintColor;
            if (GetFlag(F_SliceLeft))   style.unitySliceLeft   = m_SliceLeft;
            if (GetFlag(F_SliceTop))    style.unitySliceTop    = m_SliceTop;
            if (GetFlag(F_SliceRight))  style.unitySliceRight  = m_SliceRight;
            if (GetFlag(F_SliceBottom)) style.unitySliceBottom = m_SliceBottom;
        }
        public override void LoadFrom(RSBackground other)
        {
            m_flag        = other.m_flag;
            m_Color       = other.m_Color;
            m_Background  = other.m_Background;
            m_TintColor   = other.m_TintColor;
            m_SliceLeft   = other.m_SliceLeft;
            m_SliceTop    = other.m_SliceTop;
            m_SliceRight  = other.m_SliceRight;
            m_SliceBottom = other.m_SliceBottom;
        }
        public override void LoadFromLerp(RSBackground begin, RSBackground end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            var beginRate = 1f - rate;
            if (GetFlag(F_Color      )) m_Color       = (beginRate * begin.m_Color)     + (rate * end.m_Color);
            if (GetFlag(F_TintColor  )) m_TintColor   = (beginRate * begin.m_TintColor) + (rate * end.m_TintColor);
            if (GetFlag(F_SliceLeft  )) m_SliceLeft   = (int)((beginRate * begin.m_SliceLeft)   + (rate * end.m_SliceLeft));
            if (GetFlag(F_SliceTop   )) m_SliceTop    = (int)((beginRate * begin.m_SliceTop)    + (rate * end.m_SliceTop));
            if (GetFlag(F_SliceRight )) m_SliceRight  = (int)((beginRate * begin.m_SliceRight)  + (rate * end.m_SliceRight));
            if (GetFlag(F_SliceBottom)) m_SliceBottom = (int)((beginRate * begin.m_SliceBottom) + (rate * end.m_SliceBottom));
            if(rate >= 1f)
            {
                if (GetFlag(F_Background)) m_Background = end.m_Background;
            }
            else
            {
                if (GetFlag(F_Background)) m_Background = begin.m_Background;
            }
        }
        public override void LoadFrom(IStyle style)
        {
            SetUnsetFlag = -1;
            m_Color        = style.backgroundColor.value;
            m_Background   = style.backgroundImage.value;
            m_TintColor    = style.unityBackgroundImageTintColor.value;
            m_SliceLeft    = style.unitySliceLeft.value;
            m_SliceTop     = style.unitySliceTop.value;
            m_SliceRight   = style.unitySliceRight.value;
            m_SliceBottom  = style.unitySliceBottom.value;
        }
        public override void LoadFromIfUnset(RSBackground other)
        {
            if (!GetFlag(F_Color)       && other.GetFlag(F_Color))       m_Color       = other.m_Color;
            if (!GetFlag(F_Background)  && other.GetFlag(F_Background))  m_Background  = other.m_Background;
            if (!GetFlag(F_TintColor)   && other.GetFlag(F_TintColor))   m_TintColor   = other.m_TintColor;
            if (!GetFlag(F_SliceLeft)   && other.GetFlag(F_SliceLeft))   m_SliceLeft   = other.m_SliceLeft;
            if (!GetFlag(F_SliceTop)    && other.GetFlag(F_SliceTop))    m_SliceTop    = other.m_SliceTop;
            if (!GetFlag(F_SliceRight)  && other.GetFlag(F_SliceRight))  m_SliceRight  = other.m_SliceRight;
            if (!GetFlag(F_SliceBottom) && other.GetFlag(F_SliceBottom)) m_SliceBottom = other.m_SliceBottom;
            m_flag |= other.SetUnsetFlag;
        }

        public static Object BackgroundToObject(Background bg)
        {
            if (bg.texture != null) return bg.texture;
            else if (bg.sprite != null) return bg.sprite;
            else if (bg.renderTexture != null) return bg.renderTexture;
            else if (bg.vectorImage != null) return bg.vectorImage;
            return null;
        }
        public static Background ObjectToBackground(Object obj)
        {
            return obj switch
            {
                Texture2D img => Background.FromTexture2D(img),
                Sprite img => Background.FromSprite(img),
                RenderTexture img => Background.FromRenderTexture(img),
                VectorImage img => Background.FromVectorImage(img),
                _ => new Background(),
            };
        }
        public void OnBeforeSerialize()
        {
            s_img = BackgroundToObject(m_Background);
        }
        public void OnAfterDeserialize()
        {
            m_Background = ObjectToBackground(s_img);
            s_img = null;
        }

    }
}
