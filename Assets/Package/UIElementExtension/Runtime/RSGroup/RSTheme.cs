using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    [System.Serializable]
    public class RSTheme
    {
        public static RSTheme Current
        {
            get
            {
                if(m_current == null)
                    m_current = UIElementExtensionResource.Get.DefaultTheme.Theme.DeepCopy();
                return m_current;
            }
        }
        static RSTheme m_current = null;

        [Header("In Runtime, will layout as in Editor with (640*480) ref size.")]
        [Header("Style Settings")]
        public RSText MainText  = new();
        public RSText LabelText = new();
        public RSStyle FieldStyle  = new();

        [Header("Layout Settings")]
        public float LineHeight = 20f;
        public float LabelWidth = 180f;
        public float IndentedLabelWidth => GetIndentedLabelWidth(indentLevel);
        /// <summary>
        /// IndentWidth{ 0 : Max(IndentStep, LineHeight), other : IndentStep }
        /// </summary>
        public float IndentWidth => GetIndentWidth(indentLevel);
        public float TotalIndentWidth => GetTotalIndentWidth(indentLevel);
        public float IndentStep = 12f;
        public float VisualMargin = 4f;
        public float GetIndentWidth(int indentLevel)
        {
            if (indentLevel == 0)
                return Math.Max(IndentStep, LineHeight);
            else
                return IndentStep;
        }
        public float GetTotalIndentWidth(int indentLevel)
        {
            int cur = -1;
            float ret = 0;
            while (cur++ < indentLevel)
                ret += GetIndentWidth(cur);
            return ret;
        }
        public float GetIndentedLabelWidth(int indentLevel)
        {
            return LabelWidth - GetTotalIndentWidth(indentLevel);
        }
        public static int indentLevel = 0;

        [Header("Color Settings")]
        public RSColorSet NormalColorSet = new();
        public RSColorSet SuccessColorSet = new();
        public RSColorSet WarningColorSet = new();
        public RSColorSet DangerColorSet = new();
        public RSColorSet HintColorSet = new();

        public Color TextColor => NormalColorSet.TextColor;
        public Color BackgroundColor => NormalColorSet.BackgroundColor;
        public Color BackgroundColor2 => NormalColorSet.BackgroundColor2;
        public Color BackgroundColor3 => NormalColorSet.BackgroundColor3;
        public Color FrontgroundColor => NormalColorSet.FrontgroundColor;
        public Color FrontgroundColor2 => NormalColorSet.FrontgroundColor2;
        public Color FrontgroundColor3 => NormalColorSet.FrontgroundColor3;

        [Header("Other Settings")]
        public IconSpriteSet Icon = new();
        public CSharpSet CSharp = new ();
        public RSSize IconSize
        {
            get
            {
                return new RSSize { anySide = LineHeight };
            }
        }
        public VisualElement CreateIconElement(Sprite img,float deg = 0)
        {
            var ret = new VisualElement();
            ret.style.maxWidth = LineHeight;
            ret.style.maxHeight = LineHeight;
            ret.style.width = LineHeight;
            ret.style.height = LineHeight;
            ret.style.backgroundImage = Background.FromSprite(img);
            ret.style.unityBackgroundImageTintColor = Current.FrontgroundColor;
            ret.style.rotate = new Rotate(deg);
            return ret;
        }

        public RSTheme DeepCopy()
        {
            return new RSTheme
            {
                MainText = MainText.DeepCopy(),
                LabelText = LabelText.DeepCopy(),

                FieldStyle = FieldStyle.DeepCopy(),

                NormalColorSet = NormalColorSet.DeepCopy(),
                SuccessColorSet = SuccessColorSet.DeepCopy(),
                WarningColorSet = WarningColorSet.DeepCopy(),
                DangerColorSet = DangerColorSet.DeepCopy(),
                HintColorSet = HintColorSet.DeepCopy(),

                LineHeight = LineHeight,
                LabelWidth = LabelWidth,
                IndentStep = IndentStep,
                VisualMargin = VisualMargin,

                Icon = Icon.DeepCopy(),
                CSharp = CSharp.DeepCopy(),
            };
        }

        [System.Serializable]
        public class IconSpriteSet
        {
            public Sprite arrow;
            public Sprite earth;
            public Sprite disableEarth;
            public Sprite eyedropper;

            public IconSpriteSet DeepCopy()
            {
                return new IconSpriteSet
                {
                    arrow = arrow,
                    earth = earth,
                    disableEarth = disableEarth,
                    eyedropper = eyedropper,
                };
            }
        }

        [System.Serializable]
        public class CSharpSet
        {
            public Sprite cSharpIcon;
            public Sprite classIcon;
            public Sprite structIcon;
            public Sprite interfaceIcon;
            public Sprite enumIcon;
            public Sprite methodIcon;
            public Sprite fieldIcon;
            public Sprite propertyIcon;
            public Color methodColor     = new Color(.89f, .79f, .35f);
            public Color parameterColor  = new Color(.65f, .85f, .95f);
            public Color classColor      = new Color(.34f, .71f, .62f);
            public Color structColor     = new Color(.56f, .81f, .57f);
            public Color prefixColor     = new Color(.4f , .56f, .82f);
            public Color strColor        = new Color(.79f, .56f, .36f);
            public Color numberColor     = new Color(.6f , .8f , .6f );
            public Color controlColor    = new Color(.84f, .45f, .61f);
            public Color commentsColor   = new Color(.4f , .6f , .35f);
            public Color backgroundColor = new Color(.08f, .08f, .09f);
            public Color textColor       = new Color(.85f, .85f, .85f);

            public CSharpSet DeepCopy()
            {
                return new CSharpSet
                {
                    cSharpIcon = cSharpIcon,
                    classIcon = classIcon,
                    structIcon = structIcon,
                    interfaceIcon = interfaceIcon,
                    enumIcon = enumIcon,
                    methodIcon = methodIcon,
                    fieldIcon = fieldIcon,
                    propertyIcon = propertyIcon,

                    methodColor = methodColor,
                    parameterColor = parameterColor,
                    classColor = classColor,
                    structColor = structColor,
                    prefixColor = prefixColor,
                    strColor = strColor,
                    numberColor = numberColor,
                    controlColor = controlColor,
                    commentsColor = commentsColor,
                    backgroundColor = backgroundColor,
                    textColor = textColor,
                };
            }

            public Sprite GetTypeIcon(Type type)
            {
                if (type.IsInterface)
                    return interfaceIcon;
                if (type.IsEnum)
                    return enumIcon;
                if (type.IsClass)
                    return classIcon;
                if (type.IsValueType)
                    return structIcon;
                return cSharpIcon;
            }
            public Sprite GetMemberIcon(MemberInfo info)
            {
                if (info is Type)
                    return GetTypeIcon(info as Type);
                if (info is FieldInfo)
                    return fieldIcon; 
                if (info is PropertyInfo)
                    return propertyIcon;
                return null;
            }
        }
    }
}
