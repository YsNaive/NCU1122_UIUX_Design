using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSRadius : RSStyleComponent<RSRadius>
    {
        public const int F_TopLeft     = 1 << 0;
        public const int F_BottomLeft  = 1 << 1;
        public const int F_TopRight    = 1 << 2;
        public const int F_BottomRight = 1 << 3;
        public const int F_Any = F_TopLeft | F_BottomLeft | F_TopRight | F_BottomRight;

        public const RSLength.ModeFlag DefaultLengthMode = RSLength.ModeFlag.CanBePixel | RSLength.ModeFlag.CanBePercent | RSLength.ModeFlag.CanBeInitial | RSLength.ModeFlag.Unit;

        [SerializeField] private RSLength m_TopLeft     = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_BottomLeft  = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_TopRight    = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_BottomRight = RSLength.FromMode(DefaultLengthMode);
        #region RSLength get set
        public RSLength any
        {
            get
            {
                if (m_TopLeft == m_BottomLeft && m_TopLeft == m_TopRight && m_TopLeft == m_BottomRight)
                    return m_TopLeft;
                else return new RSLength();
            }
            set
            {
                m_TopLeft = value;
                m_BottomLeft = value;
                m_TopRight = value;
                m_BottomRight = value;
                m_flag |= F_Any;
            }
        }
        public RSLength topLeft
        {
            get => m_TopLeft;
            set
            {
                m_TopLeft = value;
                m_flag |= F_TopLeft;
            }
        }
        public RSLength bottomLeft
        {
            get => m_BottomLeft;
            set
            {
                m_BottomLeft = value;
                m_flag |= F_BottomLeft;
            }
        }
        public RSLength topRight
        {
            get => m_TopRight;
            set
            {
                m_TopRight = value;
                m_flag |= F_TopRight;
            }
        }
        public RSLength bottomRight
        {
            get => m_BottomRight;
            set
            {
                m_BottomRight = value;
                m_flag |= F_BottomRight;
            }
        }
        #endregion

        public static RSRadius Pixel(float px) { return new RSRadius { any = RSLength.Pixel(px) }; }
        public static RSRadius Percent(float percent) { return new RSRadius { any = RSLength.Percent(percent) }; }

        public override RSStyleFlag StyleFlag => RSStyleFlag.Radius;
        public override int PropertyCount => 4;

        public override void SetValueToDefault(int flag)
        {
            if((flag & F_TopLeft)     == F_TopLeft)     m_TopLeft     = RSLength.FromMode(DefaultLengthMode);
            if((flag & F_BottomLeft)  == F_BottomLeft)  m_BottomLeft  = RSLength.FromMode(DefaultLengthMode);
            if((flag & F_TopRight)    == F_TopRight)    m_TopRight    = RSLength.FromMode(DefaultLengthMode);
            if((flag & F_BottomRight) == F_BottomRight) m_BottomRight = RSLength.FromMode(DefaultLengthMode);
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_TopLeft))     style.borderTopLeftRadius = m_TopLeft;
            if (GetFlag(F_BottomLeft))  style.borderBottomLeftRadius = m_BottomLeft;
            if (GetFlag(F_TopRight))    style.borderTopRightRadius = m_TopRight;
            if (GetFlag(F_BottomRight)) style.borderBottomRightRadius = m_BottomRight;
        }
        public override void LoadFrom(RSRadius other)
        {
            m_flag = other.m_flag;
            m_TopLeft     = other.m_TopLeft;
            m_BottomLeft  = other.m_BottomLeft;
            m_TopRight    = other.m_TopRight;
            m_BottomRight = other.m_BottomRight;
        }

        public override void LoadFrom(IStyle style)
        {
            m_flag        = -1;
            m_TopLeft     = style.borderTopLeftRadius.value;
            m_BottomLeft  = style.borderBottomLeftRadius.value;
            m_TopRight    = style.borderTopRightRadius.value;
            m_BottomRight = style.borderBottomRightRadius.value;
        }

        public override void LoadFromLerp(RSRadius begin, RSRadius end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            var selfRate = 1f - rate;
            if (GetFlag(F_TopLeft))     m_TopLeft     = selfRate * begin.m_TopLeft     + rate * end.m_TopLeft;
            if (GetFlag(F_BottomLeft))  m_BottomLeft  = selfRate * begin.m_BottomLeft  + rate * end.m_BottomLeft;
            if (GetFlag(F_TopRight))    m_TopRight    = selfRate * begin.m_TopRight    + rate * end.m_TopRight;
            if (GetFlag(F_BottomRight)) m_BottomRight = selfRate * begin.m_BottomRight + rate * end.m_BottomRight;
        }

        public override void LoadFromIfUnset(RSRadius other)
        {
            if (!GetFlag(F_TopLeft)     && other.GetFlag(F_TopLeft))     m_TopLeft = other.m_TopLeft;
            if (!GetFlag(F_BottomLeft)  && other.GetFlag(F_BottomLeft))  m_BottomLeft  = other.m_BottomLeft;
            if (!GetFlag(F_TopRight)    && other.GetFlag(F_TopRight))    m_TopRight    = other.m_TopRight;
            if (!GetFlag(F_BottomRight) && other.GetFlag(F_BottomRight)) m_BottomRight = other.m_BottomRight;
            m_flag |= other.SetUnsetFlag;
        }
    }
}
