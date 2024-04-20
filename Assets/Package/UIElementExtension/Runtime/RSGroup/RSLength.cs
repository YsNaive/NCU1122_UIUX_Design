using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public struct RSLength
    {
        [System.Flags]
        public enum ModeFlag
        {
            Unit           = 1 <<  0, // T:Pixel, F:Percent
                                   
            IsAuto         = 1 <<  1,
            IsInitial      = 1 <<  2,
            IsUndefined    = 1 <<  3,
            IsNone         = 1 <<  4,
                                   
            CanBePixel     = 1 <<  6,
            CanBePercent   = 1 <<  7,
            CanBeAuto      = 1 <<  8,
            CanBeInitial   = 1 <<  9,
            CanBeNone      = 1 << 10,

            F_UnsetKeyword = ~(IsAuto  | IsInitial | IsUndefined | IsNone),
            F_CanBeAny     = (CanBePixel | CanBePercent | CanBeAuto | CanBeInitial | CanBeNone)
        }

        // define keyword/unit/editorSettings as flag
        // see ModeIndex for flag define 
        [SerializeField]
        private ModeFlag mode;
        public float value;

        /// <summary>
        /// construct a Pixel Unit with value
        /// </summary>
        public RSLength(float value)
        {
            mode = ModeFlag.F_CanBeAny | ModeFlag.IsUndefined | ModeFlag.Unit;
            this.value = value;
        }
        public RSLength(LengthUnit unit, float value)
        {
            mode = ModeFlag.F_CanBeAny | ModeFlag.IsUndefined;
            this.value = value;
            this.unit = unit;
        }
        public RSLength(StyleKeyword keyword)
        {
            mode = ModeFlag.F_CanBeAny;
            this.value = 0f;
            this.keyword = keyword;
        }

        public StyleKeyword keyword
        {
            get
            {
                if ((mode & ModeFlag.IsAuto)      == ModeFlag.IsAuto)      return StyleKeyword.Auto;
                if ((mode & ModeFlag.IsInitial)   == ModeFlag.IsInitial)   return StyleKeyword.Initial;
                if ((mode & ModeFlag.IsUndefined) == ModeFlag.IsUndefined) return StyleKeyword.Undefined;
                if ((mode & ModeFlag.IsNone)      == ModeFlag.IsNone)      return StyleKeyword.None;

                return StyleKeyword.Null;
            }
            set
            {
                mode &= ModeFlag.F_UnsetKeyword;
                mode |= value switch
                {
                    StyleKeyword.Auto      => ModeFlag.IsAuto,
                    StyleKeyword.Initial   => ModeFlag.IsInitial,
                    StyleKeyword.Undefined => ModeFlag.IsUndefined,
                    StyleKeyword.None      => ModeFlag.IsNone,
                    _                      => 0,
                };
            }
        }
        public LengthUnit unit
        {
            get
            {
                if ((mode & ModeFlag.Unit) == ModeFlag.Unit)
                    return LengthUnit.Pixel;
                else
                    return LengthUnit.Percent;
            }
            set
            {
                keyword = StyleKeyword.Undefined;
                if (value == LengthUnit.Pixel)
                    mode |= ModeFlag.Unit;
                else
                    mode &= ~ModeFlag.Unit;
            }
        }
        public ModeFlag Mode => mode;

        /// <summary>
        /// Use ths method carefully, make sure you know what you are doing.
        /// </summary>
        public void SetModeFlag(ModeFlag modeFlag)
        {
            mode = modeFlag;
        }
        public static RSLength Initial => new RSLength()
        {
            value = 0,
            mode = ModeFlag.F_CanBeAny | ModeFlag.IsInitial,
        };
        public static RSLength Auto => new RSLength()
        {
            value = 0,
            mode = ModeFlag.F_CanBeAny | ModeFlag.IsAuto,
        };
        /// <summary>
        /// Get RSLength with 100%
        /// </summary>
        public static RSLength Full => new RSLength()
        {
            value = 100,
            mode = ModeFlag.F_CanBeAny | ModeFlag.IsUndefined,
        };
        public static RSLength Pixel(float pixel) { return new RSLength() { value = pixel, mode = ModeFlag.F_CanBeAny | ModeFlag.IsUndefined | ModeFlag.Unit }; }
        public static RSLength Percent(float percent) { return new RSLength() { value = percent, mode = ModeFlag.F_CanBeAny | ModeFlag.IsUndefined }; }
        /// <summary>
        /// Use ths method carefully, make sure you know what you are doing.
        /// </summary>
        public static RSLength FromMode(ModeFlag flag, float value = 0) { return new RSLength() { mode = flag , value = value}; }

        public static RSLength Lerp(RSLength lhs, RSLength rhs, float rate)
        {
            return lhs.Lerp(rhs, rate);
        }
        public RSLength Lerp(RSLength target, float rate)
        {
            RSLength ret = this;
            ret.value = (ret.value * (1f-rate)) + (target.value*rate);
            return ret;
        }

        public override string ToString()
        {
            if (keyword == StyleKeyword.Auto)
                return "Auto";
            if (keyword == StyleKeyword.Initial)
                return "Initial";
            if (keyword == StyleKeyword.Null)
                return "Unset";

            return $"{value:.0} {(unit == LengthUnit.Pixel ? "Px" : "%")}";
        }
        public override bool Equals(object obj)
        {
            return (obj is RSLength length) && (this == length);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine((int)(value/ValueEqualDiff), mode);
        }

        #region type_op
        /// <summary>
        /// Get RSLength with Pixel
        /// </summary>
        public static implicit operator RSLength(float pixel)
        {
            return Pixel(pixel);
        }
        public static implicit operator RSLength(Length length)
        {
            return new RSLength() { value = length.value, mode = ModeFlag.F_CanBeAny | ModeFlag.IsUndefined | ((length.unit == LengthUnit.Pixel) ? ModeFlag.Unit : 0) };
        }
        public static implicit operator RSLength(StyleLength length)
        {
            RSLength result = new();
            result.value    = length.value.value;
            result.unit     = length.value.unit;
            result.keyword  = length.keyword;
            result.mode    |= ModeFlag.F_CanBeAny;
            return result;
        }
        public static implicit operator float(RSLength length)
        {
            return length.value;
        }
        public static implicit operator Length(RSLength length)
        {
            return new Length(length.value, length.unit);
        }
        public static implicit operator StyleLength(RSLength length)
        {
            if (length.keyword == StyleKeyword.Auto)
                return StyleKeyword.Auto;
            if (length.keyword == StyleKeyword.Initial)
                return StyleKeyword.Initial;

            return new StyleLength((Length)length);
        }

        #endregion

        #region value_op

        public static float ValueEqualDiff = 0.001f;
        public static bool operator !=(RSLength lhs, RSLength rhs) { return !(lhs == rhs); }
        public static bool operator ==(RSLength lhs, RSLength rhs)
        {
            if ((lhs.keyword != StyleKeyword.Undefined) && lhs.keyword == rhs.keyword)
                return true;
            if (lhs.mode != rhs.mode)
                return false;
            if (Mathf.Abs(lhs.value - rhs.value) <= ValueEqualDiff)
                return true;

            return false;
        }
        public static RSLength operator +(RSLength length, float other)
        {
            length.value += other;
            return length;
        }
        public static RSLength operator +(RSLength lhs, RSLength rhs)
        {
            lhs.value += rhs.value;
            return lhs;
        }
        public static RSLength operator -(RSLength length, float other)
        {
            length.value -= other;
            return length;
        }
        public static RSLength operator -(RSLength lhs, RSLength rhs)
        {
            lhs.value -= rhs.value;
            return lhs;
        }
        public static RSLength operator *(RSLength length, float other)
        {
            length.value *= other;
            return length;
        }
        public static RSLength operator *(RSLength lhs, RSLength rhs)
        {
            lhs.value *= rhs.value;
            return lhs;
        }
        public static RSLength operator /(RSLength length, float other)
        {
            length.value /= other;
            return length;
        }
        public static RSLength operator /(RSLength lhs, RSLength rhs)
        {
            lhs.value /= rhs.value;
            return lhs;
        }
        #endregion
    }
}
