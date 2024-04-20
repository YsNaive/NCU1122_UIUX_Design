using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSBorder : RSStyleComponent<RSBorder>
    {
        public const int F_LeftColor   = 1 << 0;
        public const int F_TopColor    = 1 << 1;
        public const int F_RightColor  = 1 << 2;
        public const int F_BottomColor = 1 << 3;
        public const int F_AnyColor = F_LeftColor | F_TopColor | F_RightColor | F_BottomColor;
        public const int F_LeftWidth   = 1 << 4;
        public const int F_TopWidth    = 1 << 5;
        public const int F_RightWidth  = 1 << 6;
        public const int F_BottomWidth = 1 << 7;
        public const int F_AnyWidth = F_LeftWidth | F_TopWidth | F_RightWidth | F_BottomWidth;
        public const int F_Any = F_AnyColor | F_AnyWidth;

        [SerializeField] private Color m_LeftColor   = Color.black;
        [SerializeField] private Color m_TopColor    = Color.black;
        [SerializeField] private Color m_RightColor  = Color.black;
        [SerializeField] private Color m_BottomColor = Color.black;
        [SerializeField] private float m_LeftWidth   = 0f;
        [SerializeField] private float m_TopWidth    = 0f;
        [SerializeField] private float m_RightWidth  = 0f;
        [SerializeField] private float m_BottomWidth = 0f;
        #region properties get set
        public Color anyColor
        {
            get
            {
                if (m_LeftColor == m_TopColor && m_LeftColor == m_RightColor && m_LeftColor == m_BottomColor)
                    return m_LeftColor;
                return Color.black;
            }
            set
            {
                m_LeftColor = value;
                m_TopColor = value;
                m_RightColor = value;
                m_BottomColor = value;
                m_flag |= F_AnyColor;
            }
        }
        public float anyWidth
        {
            get
            {
                if(m_LeftWidth == m_TopWidth && m_LeftWidth == m_RightWidth && m_LeftWidth == m_BottomWidth)
                    return m_LeftWidth;
                return 0f;
            }
            set
            {
                m_LeftWidth = value;
                m_TopWidth = value;
                m_RightWidth = value;
                m_BottomWidth = value;
                m_flag |= F_AnyWidth;
            }
        }
        public Color leftColor
        {
            get => m_LeftColor;
            set
            {
                m_LeftColor = value;
                m_flag |= F_LeftColor;
            }
        }
        public Color topColor
        {
            get => m_TopColor;
            set
            {
                m_TopColor = value;
                m_flag |= F_TopColor;
            }
        }
        public Color rightColor
        {
            get => m_RightColor;
            set
            {
                m_RightColor = value;
                m_flag |= F_RightColor;
            }
        }
        public Color bottomColor
        {
            get => m_BottomColor;
            set
            {
                m_BottomColor = value;
                m_flag |= F_BottomColor;
            }
        }
        public float leftWidth
        {
            get => m_LeftWidth;
            set
            {
                m_LeftWidth = value;
                m_flag |= F_LeftWidth;
            }
        }
        public float topWidth
        {
            get => m_TopWidth;
            set
            {
                m_TopWidth = value;
                m_flag |= F_TopWidth;
            }
        }
        public float rightWidth
        {
            get => m_RightWidth;
            set
            {
                m_RightWidth = value;
                m_flag |= F_RightWidth;
            }
        }
        public float bottomWidth
        {
            get => m_BottomWidth;
            set
            {
                m_BottomWidth = value;
                m_flag |= F_BottomWidth;
            }
        }
        #endregion

        public static readonly RSBorder Clear = new RSBorder
        {
            anyColor = Color.clear,
            anyWidth = 0,
        };

        public RSBorder() { }
        public RSBorder(float width) { anyWidth = width; }
        public RSBorder(Color color) { anyColor = color; }
        public RSBorder (Color color, float width)
        {
            anyColor = color;
            anyWidth = width;
        }

        public override RSStyleFlag StyleFlag => RSStyleFlag.Border;
        public override int PropertyCount => 8;

        public override void SetValueToDefault(int flag)
        {
            if ((flag & F_LeftColor)   == F_LeftColor)   leftColor   = Color.black;
            if ((flag & F_TopColor)    == F_TopColor)    topColor    = Color.black;
            if ((flag & F_RightColor)  == F_RightColor)  rightColor  = Color.black;
            if ((flag & F_BottomColor) == F_BottomColor) bottomColor = Color.black;
            if ((flag & F_LeftWidth)   == F_LeftWidth)   leftWidth   = 0f;
            if ((flag & F_TopWidth)    == F_TopWidth)    topWidth    = 0f;
            if ((flag & F_RightWidth)  == F_RightWidth)  rightWidth  = 0f;
            if ((flag & F_BottomWidth) == F_BottomWidth) bottomWidth = 0f;
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_LeftColor))   style.borderLeftColor   = m_LeftColor;
            if (GetFlag(F_TopColor))    style.borderTopColor    = m_TopColor;
            if (GetFlag(F_RightColor))  style.borderRightColor  = m_RightColor;
            if (GetFlag(F_BottomColor)) style.borderBottomColor = m_BottomColor;
            if (GetFlag(F_LeftWidth))   style.borderLeftWidth   = m_LeftWidth;
            if (GetFlag(F_TopWidth))    style.borderTopWidth    = m_TopWidth;
            if (GetFlag(F_RightWidth))  style.borderRightWidth  = m_RightWidth;
            if (GetFlag(F_BottomWidth)) style.borderBottomWidth = m_BottomWidth;
        }
        public override void LoadFrom(RSBorder other)
        {
            m_flag        = other.m_flag;
            m_LeftColor   = other.m_LeftColor;
            m_TopColor    = other.m_TopColor;
            m_RightColor  = other.m_RightColor;
            m_BottomColor = other.m_BottomColor;
            m_LeftWidth   = other.m_LeftWidth;
            m_TopWidth    = other.m_TopWidth;
            m_RightWidth  = other.m_RightWidth;
            m_BottomWidth = other.m_BottomWidth;
        }
        public override void LoadFrom(IStyle style)
        {
            m_flag = -1;
            m_LeftColor    = style.borderLeftColor.value;
            m_TopColor     = style.borderTopColor.value;
            m_RightColor   = style.borderRightColor.value;
            m_BottomColor  = style.borderBottomColor.value;
                         
            m_LeftWidth    = style.borderLeftWidth.value;
            m_TopWidth     = style.borderTopWidth.value;
            m_RightWidth   = style.borderRightWidth.value;
            m_BottomWidth  = style.borderBottomWidth.value;
        }
        public override void LoadFromIfUnset(RSBorder other)
        {
            if (!GetFlag(F_LeftColor)   && other.GetFlag(F_LeftColor))   m_LeftColor   = other.m_LeftColor;
            if (!GetFlag(F_TopColor)    && other.GetFlag(F_TopColor))    m_TopColor    = other.m_TopColor;
            if (!GetFlag(F_RightColor)  && other.GetFlag(F_RightColor))  m_RightColor  = other.m_RightColor;
            if (!GetFlag(F_BottomColor) && other.GetFlag(F_BottomColor)) m_BottomColor = other.m_BottomColor;
            if (!GetFlag(F_LeftWidth)   && other.GetFlag(F_LeftWidth))   m_LeftWidth   = other.m_LeftWidth;
            if (!GetFlag(F_TopWidth)    && other.GetFlag(F_TopWidth))    m_TopWidth    = other.m_TopWidth;
            if (!GetFlag(F_RightWidth)  && other.GetFlag(F_RightWidth))  m_RightWidth  = other.m_RightWidth;
            if (!GetFlag(F_BottomWidth) && other.GetFlag(F_BottomWidth)) m_BottomWidth = other.m_BottomWidth;
            m_flag |= other.SetUnsetFlag;
        }

        public override void LoadFromLerp(RSBorder begin, RSBorder end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            var beginRate = 1f - rate;
            if (GetFlag(F_LeftColor))   m_LeftColor   = (beginRate * begin.m_LeftColor)   + (rate * end.leftColor);
            if (GetFlag(F_TopColor))    m_TopColor    = (beginRate * begin.m_TopColor)    + (rate * end.topColor);
            if (GetFlag(F_RightColor))  m_RightColor  = (beginRate * begin.m_RightColor)  + (rate * end.rightColor);
            if (GetFlag(F_BottomColor)) m_BottomColor = (beginRate * begin.m_BottomColor) + (rate * end.bottomColor);
            if (GetFlag(F_LeftWidth))   m_LeftWidth   = (beginRate * begin.m_LeftWidth)   + (rate * end.leftWidth);
            if (GetFlag(F_TopWidth))    m_TopWidth    = (beginRate * begin.m_TopWidth)    + (rate * end.topWidth);
            if (GetFlag(F_RightWidth))  m_RightWidth  = (beginRate * begin.m_RightWidth)  + (rate * end.rightWidth);
            if (GetFlag(F_BottomWidth)) m_BottomWidth = (beginRate * begin.m_BottomWidth) + (rate * end.bottomWidth);
        }

    }
}
