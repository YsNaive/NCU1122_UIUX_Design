using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSPosition : RSStyleComponent<RSPosition>
    {
        public const int F_Position = 1 << 0;
        public const int F_Left     = 1 << 1;
        public const int F_Top      = 1 << 2;
        public const int F_Right    = 1 << 3;
        public const int F_Bottom   = 1 << 4;
        public const int F_Any = F_Position | F_Left | F_Top | F_Right | F_Bottom;

        const RSLength.ModeFlag DefaultLengthMode = ~RSLength.ModeFlag.CanBeNone & (RSLength.ModeFlag.F_CanBeAny | RSLength.ModeFlag.IsAuto);

        [SerializeField] private Position m_Position = Position.Relative;
        [SerializeField] private RSLength m_Left   = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_Top    = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_Right  = RSLength.FromMode(DefaultLengthMode);
        [SerializeField] private RSLength m_Bottom = RSLength.FromMode(DefaultLengthMode);
        #region properties get set
        public Position position
        {
            get => m_Position;
            set
            {
                m_Position = value;
                m_flag |= F_Position;
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
        public override RSStyleFlag StyleFlag => RSStyleFlag.Position;
        public override int PropertyCount => 5;

        public override void SetValueToDefault(int flag)
        {
            if ((flag & F_Position) == F_Position) m_Position = Position.Relative;
            if ((flag & F_Left)     == F_Left)     m_Left     = RSLength.FromMode(DefaultLengthMode);
            if ((flag & F_Top)      == F_Top)      m_Top      = RSLength.FromMode(DefaultLengthMode);
            if ((flag & F_Right)    == F_Right)    m_Right    = RSLength.FromMode(DefaultLengthMode);
            if ((flag & F_Bottom)   == F_Bottom)   m_Bottom   = RSLength.FromMode(DefaultLengthMode);
        }
        public override void ApplyOn(IStyle style)
        {
            if(GetFlag(F_Position)) style.position = m_Position;
            if(GetFlag(F_Left))     style.left     = m_Left;
            if(GetFlag(F_Top))      style.top      = m_Top;
            if(GetFlag(F_Right))    style.right    = m_Right;
            if(GetFlag(F_Bottom))   style.bottom   = m_Bottom;
        }
        public override void LoadFrom(RSPosition other)
        {
            m_flag = other.m_flag;
            m_Position = other.m_Position;
            m_Left = other.m_Left;
            m_Top = other.m_Top;
            m_Right = other.m_Right;
            m_Bottom = other.m_Bottom;
        }

        public override void LoadFrom(IStyle style)
        {
            m_flag = -1;
            m_Position = style.position.value;
            m_Left     = style.left.value;
            m_Top      = style.top.value;
            m_Right    = style.right.value;
            m_Bottom   = style.bottom.value;
        }
        public override void LoadFromLerp(RSPosition begin, RSPosition end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            if (GetFlag(F_Left))   m_Left   = begin.m_Left.Lerp(end.m_Left, rate);
            if (GetFlag(F_Top))    m_Top    = begin.m_Top.Lerp(end.m_Top, rate);
            if (GetFlag(F_Right))  m_Right  = begin.m_Right.Lerp(end.m_Right, rate);
            if (GetFlag(F_Bottom)) m_Bottom = begin.m_Bottom.Lerp(end.m_Bottom, rate);

            if (GetFlag(F_Position))
                m_Position = ((rate >= 1f) ? end : begin).m_Position;
        }
        public override void LoadFromIfUnset(RSPosition other)
        {
            if (!GetFlag(F_Position) && other.GetFlag(F_Position)) m_Position = other.m_Position;
            if (!GetFlag(F_Left)     && other.GetFlag(F_Left))     m_Left = other.m_Left;
            if (!GetFlag(F_Top)      && other.GetFlag(F_Top))      m_Top = other.m_Top;
            if (!GetFlag(F_Right)    && other.GetFlag(F_Right))    m_Right = other.m_Right;
            if (!GetFlag(F_Bottom)   && other.GetFlag(F_Bottom))   m_Bottom = other.m_Bottom;
            m_flag |= other.SetUnsetFlag;
        }
    }
}
