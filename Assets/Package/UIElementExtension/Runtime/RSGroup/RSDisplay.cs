using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSDisplay : RSStyleComponent<RSDisplay>
    {
        public const int F_Opacity    = 1 << 0;
        public const int F_Display    = 1 << 1;
        public const int F_Visibility = 1 << 2;
        public const int F_Overflow   = 1 << 3;
        public const int F_Any = F_Opacity | F_Display | F_Visibility | F_Overflow;

        [SerializeField] private float        m_Opacity    = 1f;
        [SerializeField] private DisplayStyle m_Display    = DisplayStyle.Flex;
        [SerializeField] private Visibility   m_Visibility = Visibility.Visible;
        [SerializeField] private Overflow     m_Overflow   = Overflow.Visible;
        #region properties get set
        public float opacity
        {
            get => m_Opacity;
            set
            {
                m_Opacity = value;
                m_flag |= F_Opacity;
            }
        }
        public DisplayStyle display
        {
            get => m_Display;
            set
            {
                m_Display = value;
                m_flag |= F_Display;
            }
        }
        public Visibility visibility
        {
            get => m_Visibility;
            set
            {
                m_Visibility = value;
                m_flag |= F_Visibility;
            }
        }
        public Overflow overflow
        {
            get => m_Overflow;
            set
            {
                m_Overflow = value;
                m_flag |= F_Overflow;
            }
        }
        #endregion

        public override RSStyleFlag StyleFlag => RSStyleFlag.Display;
        public override int PropertyCount => 4;

        public override void SetValueToDefault(int flag)
        {
            if ((flag & F_Opacity   ) == F_Opacity   ) m_Opacity    = 1f;
            if ((flag & F_Display   ) == F_Display   ) m_Display    = DisplayStyle.Flex;
            if ((flag & F_Visibility) == F_Visibility) m_Visibility = Visibility.Visible;
            if ((flag & F_Overflow  ) == F_Overflow  ) m_Overflow   = Overflow.Visible;
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_Opacity))    style.opacity    = m_Opacity;
            if (GetFlag(F_Display))    style.display    = m_Display;
            if (GetFlag(F_Visibility)) style.visibility = m_Visibility;
            if (GetFlag(F_Overflow))   style.overflow   = m_Overflow;
        }
        public override void LoadFrom(RSDisplay other)
        {
            m_flag       = other.m_flag;
            m_Opacity    = other.m_Opacity;
            m_Display    = other.m_Display;
            m_Visibility = other.m_Visibility;
            m_Overflow   = other.m_Overflow;
        }

        public override void LoadFrom(IStyle style)
        {
            m_flag = -1;
            m_Opacity      = style.opacity.value;
            m_Display      = style.display.value;
            m_Visibility   = style.visibility.value;
            m_Overflow     = style.overflow.value;
        }

        public override void LoadFromLerp(RSDisplay begin, RSDisplay end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            if (GetFlag(F_Opacity)) m_Opacity = ((1f - rate) * begin.m_Opacity) + (rate * end.m_Opacity);
            if(rate >= 1f)
            {
                if (GetFlag(F_Display))    m_Display = end.m_Display;
                if (GetFlag(F_Visibility)) m_Visibility = end.m_Visibility;
                if (GetFlag(F_Overflow))   m_Overflow = end.m_Overflow;
            }                                  
            else                               
            {                                  
                if (GetFlag(F_Display))    m_Display = begin.m_Display;
                if (GetFlag(F_Visibility)) m_Visibility = begin.m_Visibility;
                if (GetFlag(F_Overflow))   m_Overflow = begin.m_Overflow;
            }
        }

        public override void LoadFromIfUnset(RSDisplay other)
        {
            if (!GetFlag(F_Opacity)    && other.GetFlag(F_Opacity))    m_Opacity = other.m_Opacity;
            if (!GetFlag(F_Display)    && other.GetFlag(F_Display))    m_Display    = other.m_Display;
            if (!GetFlag(F_Visibility) && other.GetFlag(F_Visibility)) m_Visibility = other.m_Visibility;
            if (!GetFlag(F_Overflow)   && other.GetFlag(F_Overflow))   m_Overflow   = other.m_Overflow;
            m_flag |= other.SetUnsetFlag;
        }
    }
}
