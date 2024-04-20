using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public sealed class RSText : RSStyleComponent<RSText>
    {
        public const int F_FontAsset        = 1 << 0;
        public const int F_FontStyle        = 1 << 1;
        public const int F_Size             = 1 << 2;
        public const int F_Color            = 1 << 3;
        public const int F_Anchor           = 1 << 4;
        public const int F_Wrap             = 1 << 5;
        public const int F_LetterSpacing    = 1 << 6;
        public const int F_WordSpacing      = 1 << 7;
        public const int F_ParagraphSpacing = 1 << 8;
        public const int F_Any = F_FontAsset | F_FontStyle | F_Size | F_Color | F_Anchor | F_Wrap | F_LetterSpacing | F_WordSpacing | F_ParagraphSpacing;

        public const RSLength.ModeFlag DefaultLengthMode_Size    = RSLength.ModeFlag.CanBePixel | RSLength.ModeFlag.CanBePercent | RSLength.ModeFlag.CanBeInitial | RSLength.ModeFlag.IsUndefined | RSLength.ModeFlag.Unit;
        public const RSLength.ModeFlag DefaultLengthMode_Spacing = RSLength.ModeFlag.CanBePixel | RSLength.ModeFlag.CanBeInitial | RSLength.ModeFlag.IsUndefined  | RSLength.ModeFlag.Unit;

        [SerializeField] private FontAsset  m_FontAsset = null;
        [SerializeField] private FontStyle  m_FontStyle = FontStyle.Normal;
        [SerializeField] private RSLength   m_Size      = RSLength.FromMode(DefaultLengthMode_Size, 12);
        [SerializeField] private Color      m_Color     = Color.white;
        [SerializeField] private TextAnchor m_Anchor    = TextAnchor.UpperLeft;
        [SerializeField] private WhiteSpace m_Wrap      = WhiteSpace.Normal;
        [SerializeField] private RSLength m_LetterSpacing    = RSLength.FromMode(DefaultLengthMode_Spacing);
        [SerializeField] private RSLength m_WordSpacing      = RSLength.FromMode(DefaultLengthMode_Spacing);
        [SerializeField] private RSLength m_ParagraphSpacing = RSLength.FromMode(DefaultLengthMode_Spacing);
        #region RSLength get set
        public FontAsset fontAsset
        {
            get => m_FontAsset;
            set
            {
                m_FontAsset = value;
                m_flag |= F_FontAsset;
            }
        }
        public FontStyle fontStyle
        {
            get => m_FontStyle;
            set
            {
                m_FontStyle = value;
                m_flag |= F_FontStyle;
            }
        }
        public RSLength size
        {
            get => m_Size;
            set
            {
                m_Size = value;
                m_flag |= F_Size;
            }
        }
        public Color color
        {
            get => m_Color;
            set
            {
                m_Color = value;
                m_flag |= F_Color;
            }
        }
        public TextAnchor anchor
        {
            get => m_Anchor;
            set
            {
                m_Anchor = value;
                m_flag |= F_Anchor;
            }
        }
        public WhiteSpace wrap
        {
            get => m_Wrap;
            set
            {
                m_Wrap = value;
                m_flag |= F_Wrap;
            }
        }
        public RSLength letterSpacing
        {
            get => m_LetterSpacing;
            set
            {
                m_LetterSpacing = value;
                m_flag |= F_LetterSpacing;
            }
        }
        public RSLength wordSpacing
        {
            get => m_WordSpacing;
            set
            {
                m_WordSpacing = value;
                m_flag |= F_WordSpacing;
            }
        }
        public RSLength paragraphSpacing
        {
            get => m_ParagraphSpacing;
            set
            {
                m_ParagraphSpacing = value;
                m_flag |= F_ParagraphSpacing;
            }
        }
        #endregion

        public override RSStyleFlag StyleFlag => RSStyleFlag.Text;
        public override int PropertyCount => 9;

        public override void SetValueToDefault(int flag)
        {
            if ((flag & F_FontAsset) == F_FontAsset) m_FontAsset = null;
            if ((flag & F_FontStyle) == F_FontStyle) m_FontStyle = FontStyle.Normal;
            if ((flag & F_Size)      == F_Size)      m_Size      = RSLength.FromMode(DefaultLengthMode_Size, 12);
            if ((flag & F_Color)     == F_Color)     m_Color     = Color.white;
            if ((flag & F_Anchor)    == F_Anchor)    m_Anchor    = TextAnchor.UpperLeft;
            if ((flag & F_Wrap)      == F_Wrap)      m_Wrap      = WhiteSpace.Normal;
            if ((flag & F_LetterSpacing)    == F_LetterSpacing)    m_LetterSpacing    = RSLength.FromMode(DefaultLengthMode_Spacing);
            if ((flag & F_WordSpacing)      == F_WordSpacing)      m_WordSpacing      = RSLength.FromMode(DefaultLengthMode_Spacing);
            if ((flag & F_ParagraphSpacing) == F_ParagraphSpacing) m_ParagraphSpacing = RSLength.FromMode(DefaultLengthMode_Spacing);
        }
        public override void ApplyOn(IStyle style)
        {
            if (GetFlag(F_FontAsset) && m_FontAsset != null)
            {
                style.unityFont = m_FontAsset.sourceFontFile;
                style.unityFontDefinition = new StyleFontDefinition(m_FontAsset);
            }
            if (GetFlag(F_FontStyle)) style.unityFontStyleAndWeight = m_FontStyle;
            if (GetFlag(F_Size))      style.fontSize       = m_Size;
            if (GetFlag(F_Color))     style.color          = m_Color;
            if (GetFlag(F_Anchor))    style.unityTextAlign = m_Anchor;
            if (GetFlag(F_Wrap))      style.whiteSpace     = m_Wrap;
            if (GetFlag(F_LetterSpacing))    style.letterSpacing         = m_LetterSpacing;
            if (GetFlag(F_WordSpacing))      style.wordSpacing           = m_WordSpacing;
            if (GetFlag(F_ParagraphSpacing)) style.unityParagraphSpacing = m_ParagraphSpacing;
        }
        public override void LoadFrom(RSText other)
        {
            m_flag = other.m_flag;
            m_FontAsset = other.m_FontAsset;
            m_FontStyle = other.m_FontStyle;
            m_Size = other.m_Size;
            m_Color = other.m_Color;
            m_Anchor = other.m_Anchor;
            m_Wrap = other.m_Wrap;
            m_LetterSpacing = other.m_LetterSpacing;
            m_WordSpacing = other.m_WordSpacing;
            m_ParagraphSpacing = other.m_ParagraphSpacing;
        }
        public override void LoadFrom(IStyle style)
        {
            m_flag      = -1;
            m_FontAsset = style.unityFontDefinition.value.fontAsset;
            m_FontStyle = style.unityFontStyleAndWeight.value;
            m_Size      = style.fontSize;
            m_Color     = style.color.value;
            m_Anchor    = style.unityTextAlign.value;
            m_Wrap      = style.whiteSpace.value;
            m_LetterSpacing    = style.letterSpacing.value;
            m_WordSpacing      = style.wordSpacing.value;
            m_ParagraphSpacing = style.unityParagraphSpacing.value;
        }
        public override void LoadFromLerp(RSText begin, RSText end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            var beginRate = 1f - rate;
            if (GetFlag(F_Size))             m_Size  = beginRate * begin.m_Size  + rate * end.m_Size;
            if (GetFlag(F_Color))            m_Color = beginRate * begin.m_Color + rate * end.m_Color;
            if (GetFlag(F_LetterSpacing))    m_LetterSpacing    = beginRate * begin.m_LetterSpacing    + rate * end.m_LetterSpacing;
            if (GetFlag(F_WordSpacing))      m_WordSpacing      = beginRate * begin.m_WordSpacing      + rate * end.m_WordSpacing;
            if (GetFlag(F_ParagraphSpacing)) m_ParagraphSpacing = beginRate * begin.m_ParagraphSpacing + rate * end.m_ParagraphSpacing;

            var target = (rate >= 1) ? end : begin;
            if (GetFlag(F_FontAsset)) m_FontAsset = target.m_FontAsset;
            if (GetFlag(F_Anchor))    m_Anchor    = target.m_Anchor;
            if (GetFlag(F_Wrap))      m_Wrap      = target.m_Wrap;

        }
        public override void LoadFromIfUnset(RSText other)
        {
            if (!GetFlag(F_FontAsset) && other.GetFlag(F_FontAsset)) m_FontAsset = other.m_FontAsset;
            if (!GetFlag(F_FontStyle) && other.GetFlag(F_FontStyle)) m_FontStyle = other.m_FontStyle;
            if (!GetFlag(F_Size)      && other.GetFlag(F_Size))      m_Size = other.m_Size;
            if (!GetFlag(F_Color)     && other.GetFlag(F_Color))     m_Color = other.m_Color;
            if (!GetFlag(F_Anchor)    && other.GetFlag(F_Anchor))    m_Anchor = other.m_Anchor;
            if (!GetFlag(F_Wrap)      && other.GetFlag(F_Wrap))      m_Wrap = other.m_Wrap;
            if (!GetFlag(F_LetterSpacing)    && other.GetFlag(F_LetterSpacing))    m_LetterSpacing = other.m_LetterSpacing;
            if (!GetFlag(F_WordSpacing)      && other.GetFlag(F_WordSpacing))      m_WordSpacing = other.m_WordSpacing;
            if (!GetFlag(F_ParagraphSpacing) && other.GetFlag(F_ParagraphSpacing)) m_ParagraphSpacing = other.m_ParagraphSpacing;
            m_flag |= other.SetUnsetFlag;
        }
        public static int TextAnchor2Column(TextAnchor anchor)
        {
            return ((int)anchor) % 3;
        }
        public static int TextAnchor2Row(TextAnchor anchor)
        {
            return ((int)anchor) / 3;
        }

    }
}
