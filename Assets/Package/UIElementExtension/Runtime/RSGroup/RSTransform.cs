using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSTransform : RSStyleComponent<RSTransform>
    {
        public const int F_Pivot     = 1 << 0;
        public const int F_Position  = 1 << 1;
        public const int F_Scale     = 1 << 2;
        public const int F_RotateDeg = 1 << 3;
        public const int F_Any = F_Pivot | F_Position | F_Scale | F_RotateDeg;

        public const RSLength.ModeFlag DefaultLengthMode_Pivot    = RSLength.ModeFlag.CanBePixel | RSLength.ModeFlag.CanBePercent;
        public const RSLength.ModeFlag DefaultLengthMode_Position = RSLength.ModeFlag.CanBePixel | RSLength.ModeFlag.CanBePercent | RSLength.ModeFlag.Unit;

        [SerializeField] private RSLength m_PivotX = RSLength.FromMode(DefaultLengthMode_Pivot, 50f);
        [SerializeField] private RSLength m_PivotY = RSLength.FromMode(DefaultLengthMode_Pivot, 50f);
        [SerializeField] private RSLength m_x      = RSLength.FromMode(DefaultLengthMode_Position);
        [SerializeField] private RSLength m_y      = RSLength.FromMode(DefaultLengthMode_Position);
        [SerializeField] private Vector2 m_Scale   = Vector2.one;
        [SerializeField] private float m_RotateDeg = 0;
        #region RSLength get set
        public RSLength pivotX
        {
            get => m_PivotX;
            set
            {
                m_PivotX = value;
                m_flag |= F_Pivot;
            }
        }
        public RSLength pivotY
        {
            get => m_PivotY;
            set
            {
                m_PivotY = value;
                m_flag |= F_Pivot;
            }
        }
        public RSLength x
        {
            get => m_x;
            set
            {
                m_x = value;
                m_flag |= F_Position;
            }
        }
        public RSLength y
        {
            get => m_y;
            set
            {
                m_y = value;
                m_flag |= F_Position;
            }
        }
        public Vector2 scale
        {
            get => m_Scale;
            set
            {
                m_Scale = value;
                m_flag |= F_Scale;
            }
        }
        public float rotateDeg
        {
            get => m_RotateDeg;
            set
            {
                m_RotateDeg = value;
                m_flag |= F_RotateDeg;
            }
        }
        #endregion
        public override RSStyleFlag StyleFlag => RSStyleFlag.Transform;
        public override int PropertyCount => 4;

        public override void SetValueToDefault(int flag)
        {
            if((flag & F_Pivot) == F_Pivot)
            {
                m_PivotX = RSLength.FromMode(DefaultLengthMode_Pivot, 50f);
                m_PivotY = RSLength.FromMode(DefaultLengthMode_Pivot, 50f);
            }
            if((flag & F_Position) == F_Position)
            {
                m_x = RSLength.FromMode(DefaultLengthMode_Position);
                m_y = RSLength.FromMode(DefaultLengthMode_Position);
            }
            if((flag & F_Scale)     == F_Scale)     m_Scale = Vector2.one;
            if((flag & F_RotateDeg) == F_RotateDeg) m_RotateDeg = 0;
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_Pivot))     style.transformOrigin = new TransformOrigin(m_PivotX, m_PivotY);
            if (GetFlag(F_Position))  style.translate = new Translate(m_x, m_y);
            if (GetFlag(F_Scale))     style.scale     = new Scale(m_Scale);
            if (GetFlag(F_RotateDeg)) style.rotate    = new Rotate(m_RotateDeg);
        }
        public override void LoadFrom(RSTransform other)
        {
            m_flag      = other.m_flag;
            m_PivotX    = other.m_PivotX;
            m_PivotY    = other.m_PivotY;
            m_x         = other.m_x;
            m_y         = other.m_y;
            m_Scale     = other.m_Scale;
            m_RotateDeg = other.m_RotateDeg;
        }

        public override void LoadFrom(IStyle style)
        {
            m_flag      = -1;
            m_PivotX    = style.transformOrigin.value.x;
            m_PivotY    = style.transformOrigin.value.y;
            m_x         = style.translate.value.x;
            m_y         = style.translate.value.y;
            m_Scale     = style.scale.value.value;
            m_RotateDeg = style.rotate.value.angle.value;
        }

        public override void LoadFromLerp(RSTransform begin, RSTransform end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            var beginRate = 1f - rate;
            if (GetFlag(F_Pivot))
            {
                m_PivotX = begin.m_PivotX.Lerp(end.pivotX, rate);
                m_PivotY = begin.m_PivotX.Lerp(end.pivotY, rate);
            }
            if (GetFlag(F_Position))
            {
                m_x = begin.m_x.Lerp(end.m_x, rate);
                m_y = begin.m_y.Lerp(end.m_y, rate);
            }
            if (GetFlag(F_RotateDeg)) m_RotateDeg = beginRate * begin.rotateDeg + rate * end.rotateDeg;
            if (GetFlag(F_Scale))     m_Scale     = beginRate * begin.m_Scale   + rate * end.m_Scale;
        }

        public override void LoadFromIfUnset(RSTransform other)
        {
            if (!GetFlag(F_Pivot) && other.GetFlag(F_Pivot))
            {
                m_PivotX = other.m_PivotX;
                m_PivotY = other.m_PivotY;
            }
            if (!GetFlag(F_Position) && other.GetFlag(F_Position))
            {
                m_x = other.m_x;
                m_y = other.m_y;
            }
            if (!GetFlag(F_Scale)     && other.GetFlag(F_Scale))     m_Scale = other.m_Scale;
            if (!GetFlag(F_RotateDeg) && other.GetFlag(F_RotateDeg)) m_RotateDeg = other.m_RotateDeg;
            m_flag |= other.SetUnsetFlag;
        }
    }
}
