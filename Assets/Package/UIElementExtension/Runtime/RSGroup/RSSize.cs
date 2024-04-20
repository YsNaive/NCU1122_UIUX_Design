using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSSize : RSStyleComponent<RSSize>
    {
        public const int F_Width     = 1 << 0;
        public const int F_Height    = 1 << 1;
        public const int F_MinWidth  = 1 << 2;
        public const int F_MinHeight = 1 << 3;
        public const int F_MaxWidth  = 1 << 4;
        public const int F_MaxHeight = 1 << 5;
        public const int F_AnySide   = F_Width | F_Height;
        public const int F_AnyWidth  = F_Width | F_MinWidth | F_MaxWidth;
        public const int F_AnyHeight = F_Height | F_MinHeight | F_MaxHeight;
        public const int F_Any       = F_AnyWidth | F_AnyHeight;

        const RSLength.ModeFlag DefaultLengthMode_Normal = ~RSLength.ModeFlag.CanBeNone & (RSLength.ModeFlag.F_CanBeAny | RSLength.ModeFlag.Unit);
        const RSLength.ModeFlag DefaultLengthMode_Min    = ~RSLength.ModeFlag.CanBeNone & (RSLength.ModeFlag.F_CanBeAny | RSLength.ModeFlag.IsAuto);
        const RSLength.ModeFlag DefaultLengthMode_Max    = ~RSLength.ModeFlag.CanBeNone & (RSLength.ModeFlag.F_CanBeAny | RSLength.ModeFlag.IsInitial);

        [SerializeField] private RSLength m_width     = RSLength.FromMode(DefaultLengthMode_Normal);
        [SerializeField] private RSLength m_height    = RSLength.FromMode(DefaultLengthMode_Normal);
        [SerializeField] private RSLength m_minWidth  = RSLength.FromMode(DefaultLengthMode_Min);
        [SerializeField] private RSLength m_minHeight = RSLength.FromMode(DefaultLengthMode_Min);
        [SerializeField] private RSLength m_maxWidth  = RSLength.FromMode(DefaultLengthMode_Max);
        [SerializeField] private RSLength m_maxHeight = RSLength.FromMode(DefaultLengthMode_Max);

        public override RSStyleFlag StyleFlag => RSStyleFlag.Size;
        public override int PropertyCount => 6;
        #region RSLength get set
        public  RSLength anySide
        {
            get
            {
                if (m_width == m_height)
                    return m_width;
                else return RSLength.FromMode(DefaultLengthMode_Normal);
            }
            set
            {
                m_width = value;
                m_height = value;
                m_flag |= F_AnySide;
            }
        }
        public  RSLength anyHeight
        {
            get
            {
                if (m_height == m_minHeight && m_height == m_maxHeight)
                    return m_height;
                else return RSLength.FromMode(DefaultLengthMode_Normal);
            }
            set
            {
                m_height = value;
                m_minHeight = value;
                m_maxHeight = value;
                m_flag |= F_AnyHeight;
            }
        }
        public  RSLength anyWidth
        {
            get
            {
                if (m_width == m_minWidth && m_width == m_maxWidth)
                    return m_width;
                else return RSLength.FromMode(DefaultLengthMode_Normal);
            }
            set
            {
                m_width = value;
                m_minWidth = value;
                m_maxWidth = value;
                m_flag |= F_AnyWidth;
            }
        }
        public RSLength width
        {
            get => m_width;
            set
            {
                m_width = value;
                m_flag |= F_Width;
            }
        }
        public RSLength height
        {
            get => m_height;
            set
            {
                m_height = value;
                m_flag |= F_Height;
            }
        }
        public RSLength minWidth
        {
            get => m_minWidth;
            set
            {
                m_minWidth = value;
                m_flag |= F_MinWidth;
            }
        }
        public RSLength minHeight
        {
            get => m_minHeight;
            set
            {
                m_minHeight = value;
                m_flag |= F_MinHeight;
            }
        }
        public RSLength maxWidth
        {
            get => m_maxWidth;
            set
            {
                m_maxWidth = value;
                m_flag |= F_MaxWidth;
            }
        }
        public RSLength maxHeight
        {
            get => m_maxHeight;
            set
            {
                m_maxHeight = value;
                m_flag |= F_MaxHeight;
            }
        }
        #endregion
        public override void SetValueToDefault(int flag)
        {
            if ((flag & F_Width )    == F_Width)     m_width     = RSLength.FromMode(DefaultLengthMode_Normal);
            if ((flag & F_Height)    == F_Height)    m_height    = RSLength.FromMode(DefaultLengthMode_Normal);
            if ((flag & F_MinWidth)  == F_MinWidth)  m_minWidth  = RSLength.FromMode(DefaultLengthMode_Min);
            if ((flag & F_MinHeight) == F_MinHeight) m_minHeight = RSLength.FromMode(DefaultLengthMode_Min);
            if ((flag & F_MaxWidth)  == F_MaxWidth)  m_maxWidth  = RSLength.FromMode(DefaultLengthMode_Max);
            if ((flag & F_MaxHeight) == F_MaxHeight) m_maxHeight = RSLength.FromMode(DefaultLengthMode_Max);
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_Width))     style.width     = m_width;
            if (GetFlag(F_MinWidth))  style.minWidth  = m_minWidth;
            if (GetFlag(F_MaxWidth))  style.maxWidth  = m_maxWidth;
                            
            if (GetFlag(F_Height))    style.height    = m_height;
            if (GetFlag(F_MinHeight)) style.minHeight = m_minHeight;
            if (GetFlag(F_MaxHeight)) style.maxHeight = m_maxHeight;
        }
        public override void LoadFrom(RSSize other)
        {
            m_flag = other.m_flag;
            m_width     = other.m_width;
            m_height    = other.m_height;
            m_minWidth  = other.m_minWidth;
            m_minHeight = other.m_minHeight;
            m_maxWidth  = other.m_maxWidth;
            m_maxHeight = other.m_maxHeight;
        }

        public override void LoadFrom(IStyle style)
        {
            m_flag      = -1;
            m_width     = style.width;
            m_height    = style.height;
            m_minHeight = style.minHeight;
            m_minWidth  = style.minWidth;
            m_maxHeight = style.maxHeight;
            m_maxWidth  = style.maxWidth;
        }

        public override void LoadFromLerp(RSSize begin, RSSize end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            var beginRate = 1f - rate;

            if (GetFlag(F_Width))     m_width     = beginRate * begin.m_width     + rate * end.m_width;
            if (GetFlag(F_MinWidth))  m_minWidth  = beginRate * begin.m_minWidth  + rate * end.m_minWidth;
            if (GetFlag(F_MaxWidth))  m_maxWidth  = beginRate * begin.m_maxWidth  + rate * end.m_maxWidth;

            if (GetFlag(F_Height))    m_height    = beginRate * begin.m_height    + rate * end.m_height;
            if (GetFlag(F_MinHeight)) m_minHeight = beginRate * begin.m_minHeight + rate * end.m_minHeight;
            if (GetFlag(F_MaxHeight)) m_maxHeight = beginRate * begin.m_maxHeight + rate * end.m_maxHeight;
        }

        public override void LoadFromIfUnset(RSSize other)
        {
            if (!GetFlag(F_Width)     && other.GetFlag(F_Width))     m_width = other.m_width;
            if (!GetFlag(F_MinWidth)  && other.GetFlag(F_MinWidth))  m_minWidth  = other.m_minWidth;
            if (!GetFlag(F_MaxWidth)  && other.GetFlag(F_MaxWidth))  m_maxWidth  = other.m_maxWidth;
            if (!GetFlag(F_Height)    && other.GetFlag(F_Height))    m_height    = other.m_height;
            if (!GetFlag(F_MinHeight) && other.GetFlag(F_MinHeight)) m_minHeight = other.m_minHeight;
            if (!GetFlag(F_MaxHeight) && other.GetFlag(F_MaxHeight)) m_maxHeight = other.m_maxHeight;
            m_flag |= other.SetUnsetFlag;
        }
    }
}
