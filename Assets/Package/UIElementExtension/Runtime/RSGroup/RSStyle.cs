using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Flags]
    public enum RSStyleFlag
    {
        None       = 0,
        Display    = RSStyle.F_Display    ,
        Position   = RSStyle.F_Position   ,
        Flex       = RSStyle.F_Flex       ,
        Align      = RSStyle.F_Align      ,
        Size       = RSStyle.F_Size       ,
        Margin     = RSStyle.F_Margin     ,
        Padding    = RSStyle.F_Padding    ,
        Text       = RSStyle.F_Text       ,
        Background = RSStyle.F_Background ,
        Border     = RSStyle.F_Border     ,
        Radius     = RSStyle.F_Radius     ,
        Transform  = RSStyle.F_Transform  ,
        AllStyle   = RSStyle.F_Any        ,
    }

    /// <summary>
    /// If you using this class as IStyle interface, 
    /// Careful about null RSStyleComponent
    /// </summary>
    [System.Serializable]
    public sealed class RSStyle : RSStyleComponent<RSStyle>, ISerializationCallbackReceiver
    {
        #region Main Feature
        public const int F_Display    = 1 << 0;
        public const int F_Position   = 1 << 1;
        public const int F_Flex       = 1 << 2;
        public const int F_Align      = 1 << 3;
        public const int F_Size       = 1 << 4;
        public const int F_Margin     = 1 << 5;
        public const int F_Padding    = 1 << 6;
        public const int F_Text       = 1 << 7;
        public const int F_Background = 1 << 8;
        public const int F_Border     = 1 << 9;
        public const int F_Radius     = 1 << 10;
        public const int F_Transform  = 1 << 11;
        public const int F_Any = (int)(uint.MaxValue >> (32 - 12));
        

        public override int SetUnsetFlag
        {
            get => base.SetUnsetFlag;
            set
            {
                if (m_flag == value) return;
                for (int i = 0; i < 12; i++)
                {
                    var f = (1 << i);
                    SetEnable((RSStyleFlag)f, (value & f) == f);
                }
            }
        }
        public override RSStyleFlag StyleFlag => RSStyleFlag.AllStyle;
        public override int PropertyCount => 12;
        public bool GetEnable(RSStyleFlag flag) { return GetFlag((int)flag); }
        public void SetEnable(RSStyleFlag flag, bool value)
        {
            if (((flag & RSStyleFlag.Display) == RSStyleFlag.Display) && (GetEnable(RSStyleFlag.Display) != value))
            {
                if (value) m_Display = new();
                else m_Display = null;
            }
            if (((flag & RSStyleFlag.Position) == RSStyleFlag.Position) && (GetEnable(RSStyleFlag.Position) != value))
            {
                if (value) m_Position = new();
                else m_Position = null;
            }
            if (((flag & RSStyleFlag.Flex) == RSStyleFlag.Flex) && (GetEnable(RSStyleFlag.Flex) != value))
            {
                if (value) m_Flex = new();
                else m_Flex = null;
            }
            if (((flag & RSStyleFlag.Align) == RSStyleFlag.Align) && (GetEnable(RSStyleFlag.Align) != value))
            {
                if (value) m_Align = new();
                else m_Align = null;
            }
            if (((flag & RSStyleFlag.Size) == RSStyleFlag.Size) && (GetEnable(RSStyleFlag.Size) != value))
            {
                if (value) m_Size = new();
                else m_Size = null;
            }
            if (((flag & RSStyleFlag.Margin) == RSStyleFlag.Margin) && (GetEnable(RSStyleFlag.Margin) != value))
            {
                if (value) m_Margin = new();
                else m_Margin = null;
            }
            if (((flag & RSStyleFlag.Padding) == RSStyleFlag.Padding) && (GetEnable(RSStyleFlag.Padding) != value))
            {
                if (value) m_Padding = new();
                else m_Padding = null;
            }
            if (((flag & RSStyleFlag.Text) == RSStyleFlag.Text) && (GetEnable(RSStyleFlag.Text) != value))
            {
                if (value) m_Text = new();
                else m_Text = null;
            }
            if (((flag & RSStyleFlag.Background) == RSStyleFlag.Background) && (GetEnable(RSStyleFlag.Background) != value))
            {
                if (value) m_Background = new();
                else m_Background = null;
            }
            if (((flag & RSStyleFlag.Border) == RSStyleFlag.Border) && (GetEnable(RSStyleFlag.Border) != value))
            {
                if (value) m_Border = new();
                else m_Border = null;
            }
            if (((flag & RSStyleFlag.Radius) == RSStyleFlag.Radius) && (GetEnable(RSStyleFlag.Radius) != value))
            {
                if (value) m_Radius = new();
                else m_Radius = null;
            }
            if (((flag & RSStyleFlag.Transform) == RSStyleFlag.Transform) && (GetEnable(RSStyleFlag.Transform) != value))
            {
                if (value) m_Transform = new();
                else m_Transform = null;
            }
            SetFlag((int)flag, value);
        }
        public override void ApplyOn(IStyle style)
        {
            foreach (var component in VisitActiveStyle())
                component.ApplyOn(style);
        }
        public override void LoadFrom(IStyle style)
        {
            SetUnsetFlag = int.MaxValue;
            foreach (var component in VisitActiveStyle())
                component.LoadFrom(style);
        }
        public IEnumerable<RSStyleComponent> VisitActiveStyle()
        {
            if (GetEnable(RSStyleFlag.Display))    yield return m_Display;
            if (GetEnable(RSStyleFlag.Position))   yield return m_Position;
            if (GetEnable(RSStyleFlag.Flex))       yield return m_Flex;
            if (GetEnable(RSStyleFlag.Align))      yield return m_Align;
            if (GetEnable(RSStyleFlag.Size))       yield return m_Size;
            if (GetEnable(RSStyleFlag.Margin))     yield return m_Margin;
            if (GetEnable(RSStyleFlag.Padding))    yield return m_Padding;
            if (GetEnable(RSStyleFlag.Text))       yield return m_Text;
            if (GetEnable(RSStyleFlag.Background)) yield return m_Background;
            if (GetEnable(RSStyleFlag.Border))     yield return m_Border;
            if (GetEnable(RSStyleFlag.Radius))     yield return m_Radius;
            if (GetEnable(RSStyleFlag.Transform))  yield return m_Transform;
        }
        public RSStyleComponent GetStyleComponent(RSStyleFlag styleFlagIndex)
        {
            foreach(var com in VisitActiveStyle())
            {
                if(com.StyleFlag == styleFlagIndex) 
                    return com;
            }
            return null;
        }
        public override void SetValueToDefault(int flag)
        {
            foreach(var component in VisitActiveStyle())
            {
                if (((RSStyleFlag)m_flag & component.StyleFlag) == component.StyleFlag)
                    component.SetAllValueToDefault();
            }
        }
        public override void LoadFrom(RSStyle other)
        {
            SetUnsetFlag = 0;
            SetUnsetFlag = other.m_flag;
            if (GetEnable(RSStyleFlag.Display)   ) m_Display.LoadFrom(other.m_Display);
            if (GetEnable(RSStyleFlag.Position)  ) m_Position.LoadFrom(other.m_Position);
            if (GetEnable(RSStyleFlag.Flex)      ) m_Flex.LoadFrom(other.m_Flex);
            if (GetEnable(RSStyleFlag.Align)     ) m_Align.LoadFrom(other.m_Align);
            if (GetEnable(RSStyleFlag.Size)      ) m_Size.LoadFrom(other.m_Size);
            if (GetEnable(RSStyleFlag.Margin)    ) m_Margin.LoadFrom(other.m_Margin);
            if (GetEnable(RSStyleFlag.Padding)   ) m_Padding.LoadFrom(other.m_Padding);
            if (GetEnable(RSStyleFlag.Text)      ) m_Text.LoadFrom(other.m_Text);
            if (GetEnable(RSStyleFlag.Background)) m_Background.LoadFrom(other.m_Background);
            if (GetEnable(RSStyleFlag.Border)    ) m_Border.LoadFrom(other.m_Border);
            if (GetEnable(RSStyleFlag.Radius)    ) m_Radius.LoadFrom(other.m_Radius);
            if (GetEnable(RSStyleFlag.Transform) ) m_Transform.LoadFrom(other.m_Transform);
        }
        public override void LoadFromLerp(RSStyle begin, RSStyle end, float rate)
        {
            m_flag = begin.m_flag & end.m_flag;
            if (GetEnable(RSStyleFlag.Display))
                m_Display.LoadFromLerp(begin.m_Display, end.m_Display, rate);
            if (GetEnable(RSStyleFlag.Position))
                m_Position.LoadFromLerp(begin.m_Position, end.m_Position, rate);
            if (GetEnable(RSStyleFlag.Flex))
                m_Flex.LoadFromLerp(begin.m_Flex, end.m_Flex, rate);
            if (GetEnable(RSStyleFlag.Align))
                m_Align.LoadFromLerp(begin.m_Align, end.m_Align, rate);
            if (GetEnable(RSStyleFlag.Size))
                m_Size.LoadFromLerp(begin.m_Size, end.m_Size, rate);
            if (GetEnable(RSStyleFlag.Margin))
                m_Margin.LoadFromLerp(begin.m_Margin, end.m_Margin, rate);
            if (GetEnable(RSStyleFlag.Padding))
                m_Padding.LoadFromLerp(begin.m_Padding, end.m_Padding, rate);
            if (GetEnable(RSStyleFlag.Text))
                m_Text.LoadFromLerp(begin.m_Text, end.m_Text, rate);
            if (GetEnable(RSStyleFlag.Background))
                m_Background.LoadFromLerp(begin.m_Background, end.m_Background, rate);
            if (GetEnable(RSStyleFlag.Border))
                m_Border.LoadFromLerp(begin.m_Border, end.m_Border, rate);
            if (GetEnable(RSStyleFlag.Radius))
                m_Radius.LoadFromLerp(begin.m_Radius, end.m_Radius, rate);
            if (GetEnable(RSStyleFlag.Transform))
                m_Transform.LoadFromLerp(begin.m_Transform, end.m_Transform, rate);
        }
        public override void LoadFromIfUnset(RSStyle other)
        {
            SetUnsetFlag |= other.SetUnsetFlag;
            foreach (var com in VisitActiveStyle())
            {
                var otherCom = other.GetStyleComponent(com.StyleFlag);
                if(otherCom != null)
                    com.LoadFromIfUnset(otherCom);
            }
        }

        [SerializeField] RSStyleCompressSerializer s_data;
        public void OnBeforeSerialize()
        {
            s_data ??= new();
            s_data.StyleComponents = new() { this };
        }
        public void OnAfterDeserialize()
        {
            if ((s_data?.StyleComponents?.Count ?? -1) > 0)
                LoadFrom((RSStyle)s_data.StyleComponents[0]);
            else
                SetUnsetFlag = 0;
            s_data = null;
        }
        #endregion

        #region Components GET/SET & Serialize field

        [System.NonSerialized] private RSDisplay    m_Display;
        [System.NonSerialized] private RSPosition   m_Position;
        [System.NonSerialized] private RSFlex       m_Flex;
        [System.NonSerialized] private RSAlign      m_Align;
        [System.NonSerialized] private RSSize       m_Size;
        [System.NonSerialized] private RSMargin     m_Margin;
        [System.NonSerialized] private RSPadding    m_Padding;
        [System.NonSerialized] private RSText       m_Text;
        [System.NonSerialized] private RSBackground m_Background;
        [System.NonSerialized] private RSBorder     m_Border;
        [System.NonSerialized] private RSRadius     m_Radius;
        [System.NonSerialized] private RSTransform  m_Transform;

        public RSDisplay Display
        {
            get => m_Display;
            set
            {
                if (m_Display != value)
                {
                    m_Display = value;
                    SetFlag((int)RSStyleFlag.Display, value != null);
                }
            }
        }
        public RSPosition Position
        {
            get => m_Position;
            set
            {
                if (m_Position != value)
                {
                    m_Position = value;
                    SetFlag((int)RSStyleFlag.Position, value != null);
                }
            }
        }
        public RSFlex Flex
        {
            get => m_Flex;
            set
            {
                if (m_Flex != value)
                {
                    m_Flex = value;
                    SetFlag((int)RSStyleFlag.Flex, value != null);
                }
            }
        }
        public RSAlign Align
        {
            get => m_Align;
            set
            {
                if (m_Align != value)
                {
                    m_Align = value;
                    SetFlag((int)RSStyleFlag.Align, value != null);
                }
            }
        }
        public RSSize Size
        {
            get => m_Size;
            set
            {
                if (m_Size != value)
                {
                    m_Size = value;
                    SetFlag((int)RSStyleFlag.Size, value != null);
                }
            }
        }
        public RSMargin Margin
        {
            get => m_Margin;
            set
            {
                if (m_Margin != value)
                {
                    m_Margin = value;
                    SetFlag((int)RSStyleFlag.Margin, value != null);
                }
            }
        }
        public RSPadding Padding
        {
            get => m_Padding;
            set
            {
                if (m_Padding != value)
                {
                    m_Padding = value;
                    SetFlag((int)RSStyleFlag.Padding, value != null);
                }
            }
        }
        public RSText Text
        {
            get => m_Text;
            set
            {
                if (m_Text != value)
                {
                    m_Text = value;
                    SetFlag((int)RSStyleFlag.Text, value != null);
                }
            }
        }
        public RSBackground Background
        {
            get => m_Background;
            set
            {
                if (m_Background != value)
                {
                    m_Background = value;
                    SetFlag((int)RSStyleFlag.Background, value != null);
                }
            }
        }
        public RSBorder Border
        {
            get => m_Border;
            set
            {
                if (m_Border != value)
                {
                    m_Border = value;
                    SetFlag((int)RSStyleFlag.Border, value != null);
                }
            }
        }
        public RSRadius Radius
        {
            get => m_Radius;
            set
            {
                if (m_Radius != value)
                {
                    m_Radius = value;
                    SetFlag((int)RSStyleFlag.Radius, value != null);
                }
            }
        }
        public RSTransform Transform
        {
            get => m_Transform;
            set
            {
                if (m_Transform != value)
                {
                    m_Transform = value;
                    SetFlag((int)RSStyleFlag.Transform, value != null);
                }
            }
        }
        #endregion

        #region Static Method
        public static RSStyle CreateReleatedAnchorLayout(TextAnchor anchor) { return CreateReleatedAnchorLayout(anchor, RSLength.Pixel(0)); }
        public static RSStyle CreateReleatedAnchorLayout(TextAnchor anchor, RSLength padding)
        {
            RSStyle ret = new RSStyle() {SetUnsetFlag = F_Position | F_Padding | F_Margin };
            var col = RSText.TextAnchor2Column(anchor);
            var row = RSText.TextAnchor2Row(anchor);
            RSLength empty = RSLength.Pixel(0);
            RSLength auto = RSLength.Auto;
            if (col == 0)
            {
                ret.Position.left = empty;
                ret.Padding.left = padding;
                ret.Margin.left = empty;
                ret.Position.right = auto;
                ret.Padding.right = auto;
                ret.Margin.right = auto;
            }
            else if (col == 1)
            {
                ret.Position.left = auto;
                ret.Padding.left = auto;
                ret.Margin.left = auto;
                ret.Position.right = auto;
                ret.Padding.right = auto;
                ret.Margin.right = auto;
            }
            else
            {
                ret.Position.left = auto;
                ret.Padding.left = auto;
                ret.Margin.left = auto;
                ret.Position.right = empty;
                ret.Padding.right = padding;
                ret.Margin.right = empty;
            }

            if (row == 0)
            {
                ret.Position.top = empty;
                ret.Padding.top = padding;
                ret.Margin.top = empty;
                ret.Position.bottom = auto;
                ret.Padding.bottom = auto;
                ret.Margin.bottom = auto;
            }
            else if (row == 1)
            {
                ret.Position.top = auto;
                ret.Padding.top = auto;
                ret.Margin.top = auto;
                ret.Position.bottom = auto;
                ret.Padding.bottom = auto;
                ret.Margin.bottom = auto;
            }
            else
            {
                ret.Position.top = auto;
                ret.Padding.top = auto;
                ret.Margin.top = auto;
                ret.Position.bottom = empty;
                ret.Padding.bottom = padding;
                ret.Margin.bottom = empty;
            }

            return ret;
        }

        #endregion

        /*
        #region interface IStyle

        static System.Exception notSupportExcption = new System.Exception("RS Style set not support this property.");
        public StyleEnum<Align> alignContent { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleEnum<Align> alignItems { get => m_Align.AlignItems; set => m_Align.AlignItems = value.value; }
        public StyleEnum<Align> alignSelf { get => m_Align.AlignSelf; set => m_Align.AlignSelf = value.value; }
        public StyleColor backgroundColor { get => m_Background.Color; set => m_Background.Color = value.value; }
        public StyleBackground backgroundImage { get => m_Background.Background; set => m_Background.Background = value.value; }
        public StyleColor borderBottomColor { get => m_Border.BottomColor; set => m_Border.BottomColor = value.value; }
        public StyleLength borderBottomLeftRadius { get => m_Radius.BottomLeft; set => m_Radius.BottomLeft = value.value; }
        public StyleLength borderBottomRightRadius { get => m_Radius.BottomRight; set => m_Radius.BottomRight = value.value; }
        public StyleFloat borderBottomWidth { get => m_Border.BottomWidth; set => m_Border.BottomWidth = value.value; }
        public StyleColor borderLeftColor { get => m_Border.LeftColor; set => m_Border.LeftColor = value.value; }
        public StyleFloat borderLeftWidth { get => m_Border.LeftWidth; set => m_Border.LeftWidth = value.value; }
        public StyleColor borderRightColor { get => m_Border.RightColor; set => m_Border.RightColor = value.value; }
        public StyleFloat borderRightWidth { get => m_Border.RightWidth; set => m_Border.RightWidth = value.value; }
        public StyleColor borderTopColor { get => m_Border.TopColor; set => m_Border.TopColor = value.value; }
        public StyleLength borderTopLeftRadius { get => m_Radius.TopLeft; set => m_Radius.TopLeft = value.value; }
        public StyleLength borderTopRightRadius { get => m_Radius.TopRight; set => m_Radius.TopRight = value.value; }
        public StyleFloat borderTopWidth { get => m_Border.TopWidth; set => m_Border.TopWidth = value.value; }
        public StyleLength bottom { get => m_Position.Bottom; set => m_Position.Bottom = value.value; }
        public StyleColor color { get => m_Text.Color; set => m_Text.Color = value.value; }
        public StyleCursor cursor { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleEnum<DisplayStyle> display { get => m_Display.Display; set => m_Display.Display = display.value; }
        public StyleLength flexBasis { get => m_Flex.Basis; set => m_Flex.Basis = value.value; }
        public StyleEnum<FlexDirection> flexDirection { get => m_Flex.FlexDirection; set => m_Flex.FlexDirection = value.value; }
        public StyleFloat flexGrow { get => m_Flex.Grow; set => m_Flex.Grow = value.value; }
        public StyleFloat flexShrink { get => m_Flex.Shrink; set => m_Flex.Shrink = value.value; }
        public StyleEnum<Wrap> flexWrap { get => m_Flex.Wrap; set => m_Flex.Wrap = value.value; }
        public StyleLength fontSize { get => m_Text.Size; set => m_Text.Size = value.value; }
        public StyleLength height { get => m_Size.height; set => m_Size.height = value.value; }
        public StyleEnum<Justify> justifyContent { get => m_Align.JustifyContent; set => m_Align.JustifyContent = value.value; }
        public StyleLength left { get => m_Position.Left; set => m_Position.Left = value.value; }
        public StyleLength letterSpacing { get => m_Text.LetterSpacing; set => m_Text.LetterSpacing = value.value; }
        public StyleLength marginBottom { get => m_Margin.Bottom; set => m_Margin.Bottom = value.value; }
        public StyleLength marginLeft { get => m_Margin.Left; set => m_Margin.Left = value.value; }
        public StyleLength marginRight { get => m_Margin.Right; set => m_Margin.Right = value.value; }
        public StyleLength marginTop { get => m_Margin.Top; set => m_Margin.Top = value.value; }
        public StyleLength maxHeight { get => m_Size.maxHeight; set => m_Size.maxHeight = value.value; }
        public StyleLength maxWidth { get => m_Size.maxWidth; set => m_Size.maxWidth = value.value; }
        public StyleLength minHeight { get => m_Size.minHeight; set => m_Size.minHeight = value.value; }
        public StyleLength minWidth { get => m_Size.minWidth; set => m_Size.minWidth = value.value; }
        public StyleFloat opacity { get => m_Display.Opacity; set => m_Display.Opacity = value.value; }
        public StyleEnum<Overflow> overflow { get => m_Display.Overflow; set => m_Display.Overflow = value.value; }
        public StyleLength paddingBottom { get => m_Padding.Bottom; set => m_Padding.Bottom = value.value; }
        public StyleLength paddingLeft { get => m_Padding.Left; set => m_Padding.Left = value.value; }
        public StyleLength paddingRight { get => m_Padding.Right; set => m_Padding.Right = value.value; }
        public StyleLength paddingTop { get => m_Padding.Top; set => m_Padding.Top = value.value; }
        public StyleEnum<Position> position { get => m_Position.Position; set => m_Position.Position = value.value; }
        public StyleLength right { get => m_Position.Right; set => m_Position.Right = value.value; }
        public StyleRotate rotate { get => new Rotate(m_Transform.RotateDeg); set => m_Transform.RotateDeg = value.value.angle.ToDegrees(); }
        public StyleScale scale { get => new Scale(m_Transform.Scale); set => m_Transform.Scale = value.value.value; }
        public StyleEnum<TextOverflow> textOverflow { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleTextShadow textShadow { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleLength top { get => m_Position.Top; set => m_Position.Top = value.value; }
        public StyleTransformOrigin transformOrigin { get => new TransformOrigin(m_Transform.x, m_Transform.y); set { m_Transform.x = value.value.x; m_Transform.y = value.value.y; } }
        public StyleList<TimeValue> transitionDelay { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleList<TimeValue> transitionDuration { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleList<StylePropertyName> transitionProperty { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleList<EasingFunction> transitionTimingFunction { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleTranslate translate { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleColor unityBackgroundImageTintColor { get => m_Background.TintColor; set => m_Background.TintColor = value.value; }
        public StyleEnum<ScaleMode> unityBackgroundScaleMode { get => m_Background.ScaleMode; set => m_Background.ScaleMode = value.value; }
        public StyleFont unityFont { get => new StyleFont(m_Text.FontAsset.sourceFontFile); set => throw notSupportExcption; }
        public StyleFontDefinition unityFontDefinition { get => new FontDefinition() { fontAsset = m_Text.FontAsset }; set => m_Text.FontAsset = value.value.fontAsset; }
        public StyleEnum<FontStyle> unityFontStyleAndWeight { get => m_Text.FontStyle; set => m_Text.FontStyle = value.value; }
        public StyleEnum<OverflowClipBox> unityOverflowClipBox { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleLength unityParagraphSpacing { get => m_Text.ParagraphSpacing; set => m_Text.ParagraphSpacing = value.value; }
        public StyleInt unitySliceBottom { get => m_Background.SliceBottom; set => m_Background.SliceBottom = value.value; }
        public StyleInt unitySliceLeft { get => m_Background.SliceLeft; set => m_Background.SliceLeft = value.value; }
        public StyleInt unitySliceRight { get => m_Background.SliceRight; set => m_Background.SliceRight = value.value; }
        public StyleInt unitySliceTop { get => m_Background.SliceTop; set => m_Background.SliceTop = value.value; }
        public StyleEnum<TextAnchor> unityTextAlign { get => m_Text.Anchor; set => m_Text.Anchor = value.value; }
        public StyleColor unityTextOutlineColor { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleFloat unityTextOutlineWidth { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleEnum<TextOverflowPosition> unityTextOverflowPosition { get => throw notSupportExcption; set => throw notSupportExcption; }
        public StyleEnum<Visibility> visibility { get => m_Display.Visibility; set => m_Display.Visibility = value.value; }
        public StyleEnum<WhiteSpace> whiteSpace { get => m_Text.Wrap; set => m_Text.Wrap = value.value; }
        public StyleLength width { get => m_Size.width; set => m_Size.width = value.value; }
        public StyleLength wordSpacing { get => m_Text.WordSpacing; set => m_Text.WordSpacing = value.value; }
        #endregion

        */
    }
}
