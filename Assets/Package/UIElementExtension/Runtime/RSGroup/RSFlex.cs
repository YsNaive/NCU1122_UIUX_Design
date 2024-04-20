using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSFlex : RSStyleComponent<RSFlex>
    {
        public const int F_Basis     = 1 << 0;
        public const int F_Shrink    = 1 << 1;
        public const int F_Grow      = 1 << 2;
        public const int F_Direction = 1 << 3;
        public const int F_Wrap      = 1 << 4;
        public const int F_Any = F_Basis | F_Shrink | F_Grow | F_Direction | F_Wrap;

        const RSLength.ModeFlag DefaultLengthMode_Basis = ~RSLength.ModeFlag.CanBeNone & (RSLength.ModeFlag.F_CanBeAny | RSLength.ModeFlag.IsAuto);

        [SerializeField] private RSLength m_Basis  = RSLength.FromMode(DefaultLengthMode_Basis);
        [SerializeField] private float    m_Shrink = 0;
        [SerializeField] private float    m_Grow   = 0;
        [SerializeField] private FlexDirection m_Direction = FlexDirection.Column;
        [SerializeField] private Wrap     m_Wrap = Wrap.Wrap;
        #region properties get set
        public RSLength basis
        {
            get => m_Basis;
            set
            {
                m_Basis = value;
                m_flag |= F_Basis;
            }
        }
        public float shrink
        {
            get => m_Shrink;
            set
            {
                m_Shrink = value;
                m_flag |= F_Shrink;
            }
        }
        public float grow
        {
            get => m_Grow;
            set
            {
                m_Grow = value;
                m_flag |= F_Grow;
            }
        }
        public FlexDirection direction
        {
            get => m_Direction;
            set
            {
                m_Direction = value;
                m_flag |= F_Direction;
            }
        }
        public Wrap wrap
        {
            get => m_Wrap;
            set
            {
                m_Wrap = value;
                m_flag |= F_Wrap;
            }
        }
        #endregion

        #region static shortcut
        public static readonly RSFlex Horizontal = new RSFlex { direction = FlexDirection.Row, };
        public static readonly RSFlex Vertical = new RSFlex { direction = FlexDirection.Column, };
        #endregion

        public override RSStyleFlag StyleFlag => RSStyleFlag.Flex;
        public override int PropertyCount => 5;

        public override void SetValueToDefault(int flag)
        {
            if ((flag & F_Basis)         == F_Basis)         m_Basis  = RSLength.FromMode(DefaultLengthMode_Basis);
            if ((flag & F_Shrink)        == F_Shrink)        m_Shrink = 0;
            if ((flag & F_Grow)          == F_Grow)          m_Grow   = 0;
            if ((flag & F_Direction) == F_Direction) m_Direction = FlexDirection.Column;
            if ((flag & F_Wrap)          == F_Wrap)          m_Wrap = Wrap.Wrap;
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_Basis))         style.flexBasis     = m_Basis;
            if (GetFlag(F_Shrink))        style.flexShrink    = m_Shrink;
            if (GetFlag(F_Grow))          style.flexGrow      = m_Grow;
            if (GetFlag(F_Direction))     style.flexDirection = m_Direction;
            if (GetFlag(F_Wrap))          style.flexWrap      = m_Wrap;
        }
        public override void LoadFrom(RSFlex other)
        {                
            m_flag   = other.m_flag;
            m_Basis  = other.m_Basis;
            m_Shrink = other.m_Shrink;
            m_Grow   = other.m_Grow;
            m_Direction = other.m_Direction;
            m_Wrap   = other.m_Wrap;
        }

        public override void LoadFrom(IStyle style)
        {
            m_flag  = -1;
            m_Basis     = style.flexBasis.value;
            m_Shrink    = style.flexShrink.value;
            m_Grow      = style.flexGrow.value;
            m_Direction = style.flexDirection.value;
            m_Wrap      = style.flexWrap.value;
        }

        public override void LoadFromLerp(RSFlex begin, RSFlex end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            var selfRate = 1f - rate;
            if (GetFlag(F_Basis))  m_Basis  = begin.m_Basis.Lerp(end.m_Basis, rate);
            if (GetFlag(F_Shrink)) m_Shrink = selfRate * begin.m_Shrink + end.m_Shrink * rate; 
            if (GetFlag(F_Grow))   m_Grow   = selfRate * begin.m_Grow   + end.m_Grow   * rate;
            if(rate >= 0.5f)
            {
                if (GetFlag(F_Direction)) m_Direction = end.m_Direction;
                if (GetFlag(F_Wrap))           m_Wrap = end.m_Wrap;
            }                                                           
            else                                                        
            {                                                           
                if (GetFlag(F_Direction)) m_Direction = begin.m_Direction;
                if (GetFlag(F_Wrap))           m_Wrap = begin.m_Wrap;
            }
        }

        public override void LoadFromIfUnset(RSFlex other)
        {
            if (!GetFlag(F_Basis)     && other.GetFlag(F_Basis))     m_Basis     = other.m_Basis;
            if (!GetFlag(F_Shrink)    && other.GetFlag(F_Shrink))    m_Shrink    = other.m_Shrink;
            if (!GetFlag(F_Grow)      && other.GetFlag(F_Grow))      m_Grow      = other.m_Grow;
            if (!GetFlag(F_Direction) && other.GetFlag(F_Direction)) m_Direction = other.m_Direction;
            if (!GetFlag(F_Wrap)      && other.GetFlag(F_Wrap))      m_Wrap      = other.m_Wrap;
            m_flag |= other.SetUnsetFlag;
        }
    }
}
