using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSPadding : RSStyleComponent<RSPadding>
    {
        public const int F_Left = 1 << 0;
        public const int F_Top = 1 << 1;
        public const int F_Right = 1 << 2;
        public const int F_Bottom = 1 << 3;
        public const int F_Any = F_Left | F_Top | F_Right | F_Bottom;

        public const RSLength.ModeFlag DefaultLengthMode = ~RSLength.ModeFlag.CanBeNone & (RSLength.ModeFlag.F_CanBeAny | RSLength.ModeFlag.Unit);

        [SerializeField] private RSLength m_Left = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_Top = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_Right = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_Bottom = RSLength.FromMode(DefaultLengthMode);
        #region properties get set
        public RSLength any
        {
            get
            {
                if (m_Left == m_Top && m_Left == m_Right && m_Left == m_Bottom)
                    return m_Left;
                else return RSLength.FromMode(DefaultLengthMode);
            }
            set
            {
                m_Left = value;
                m_Top = value;
                m_Right = value;
                m_Bottom = value;
                m_flag |= F_Any;
            }
        }
        public RSLength left
        {
            get => m_Left;
            set
            {
                m_Left = value;
                m_flag |= F_Left;
            }
        }
        public RSLength top
        {
            get => m_Top;
            set
            {
                m_Top = value;
                m_flag |= F_Top;
            }
        }
        public RSLength right
        {
            get => m_Right;
            set
            {
                m_Right = value;
                m_flag |= F_Right;
            }
        }
        public RSLength bottom
        {
            get => m_Bottom;
            set
            {
                m_Bottom = value;
                m_flag |= F_Bottom;
            }
        }
        #endregion

        #region static shortcut
        public static readonly RSPadding Clear = new RSPadding() { any = RSLength.Pixel(0) };
        #endregion

        public override RSStyleFlag StyleFlag => RSStyleFlag.Padding;
        public override int PropertyCount => 4;

        public override void SetValueToDefault(int flag)
        {
            if((flag & F_Left)   == F_Left)   m_Left   = RSLength.FromMode(DefaultLengthMode);
            if((flag & F_Top)    == F_Top)    m_Top    = RSLength.FromMode(DefaultLengthMode);
            if((flag & F_Right)  == F_Right)  m_Right  = RSLength.FromMode(DefaultLengthMode);
            if((flag & F_Bottom) == F_Bottom) m_Bottom = RSLength.FromMode(DefaultLengthMode);
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_Left))   style.paddingLeft   = m_Left;
            if (GetFlag(F_Top))    style.paddingTop    = m_Top;
            if (GetFlag(F_Right))  style.paddingRight  = m_Right;
            if (GetFlag(F_Bottom)) style.paddingBottom = m_Bottom;
        }
        public override void LoadFrom(RSPadding other)
        {
            m_flag = other.m_flag;
            m_Left = other.m_Left;
            m_Top = other.m_Top;
            m_Right = other.m_Right;
            m_Bottom = other.m_Bottom;
        }
        public override void LoadFrom(IStyle style)
        {
            m_flag = -1;
            m_Left   = style.paddingLeft.value;
            m_Top    = style.paddingTop.value;
            m_Right  = style.paddingRight.value;
            m_Bottom = style.paddingBottom.value;
        }
        public override void LoadFromLerp(RSPadding begin, RSPadding end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            if (GetFlag(F_Left))   m_Left   = begin.m_Left.Lerp(end.m_Left, rate);
            if (GetFlag(F_Top))    m_Top    = begin.m_Top.Lerp(end.m_Top, rate);
            if (GetFlag(F_Right))  m_Right  = begin.m_Right.Lerp(end.m_Right, rate);
            if (GetFlag(F_Bottom)) m_Bottom = begin.m_Bottom.Lerp(end.m_Bottom, rate);
        }
        public override void LoadFromIfUnset(RSPadding other)
        {
            if (!GetFlag(F_Left)   && other.GetFlag(F_Left))   m_Left = other.m_Left;
            if (!GetFlag(F_Top)    && other.GetFlag(F_Top))    m_Top = other.m_Top;
            if (!GetFlag(F_Right)  && other.GetFlag(F_Right))  m_Right = other.m_Right;
            if (!GetFlag(F_Bottom) && other.GetFlag(F_Bottom)) m_Bottom = other.m_Bottom;
            m_flag |= other.SetUnsetFlag;
        }
    }
}
