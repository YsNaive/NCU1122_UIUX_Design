using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public interface IRuntimeDrawer
    {
        //public RuntimeDrawerLayoutMode LayoutMode { get; set; }
        public event Action OnValueChanged;
        public event Action<RuntimeDrawer> OnMemberValueChanged;
        public void SetValue(object value);
        public object GetValue();
        public void SetValueWithoutNotify(object newValue);
        public void RepaintDrawer();
        public void ReciveAttribute(Attribute attribute);
    }
    public interface IRuntimeDrawerDecorator
    {
        public Type RequiredAttribute { get; }
        public void DecorateDrawer(Attribute attribute, RuntimeDrawer drawer);
    }
    public abstract class RuntimeDrawer : VisualElement, IRuntimeDrawer
    {
        #region static
        public static int MaxNestLevel = 12;
        public static IEnumerable<(CustomRuntimeDrawerAttribute attribute, Type drawerType)> ActiveDrawer
        {
            get
            {
                foreach(var drawer in m_ActiveDrawer)
                    yield return drawer;
            }
        }
        private static List<(CustomRuntimeDrawerAttribute attribute, Type drawerType)> m_ActiveDrawer;
        private static readonly Dictionary<Type, IRuntimeDrawerDecorator> DecoratorDict = new();
        static RuntimeDrawer()
        {
            m_ActiveDrawer = TypeReader
                .FindAllTypesWhere(type => !type.IsAbstract
                                        && typeof(IRuntimeDrawer).IsAssignableFrom(type)
                                        && type.IsDefined(typeof(CustomRuntimeDrawerAttribute), false))
                .Select (type => (type.GetCustomAttribute<CustomRuntimeDrawerAttribute>(), type))
                .OrderBy(pair => -pair.Item1.Priority)
                .ToList();
            foreach (var pair in m_ActiveDrawer)
            {
                drawerType2Attribute.Add(pair.drawerType, pair.attribute);
                if(pair.attribute.RequiredAttribute != null)
                    drawerMatchDict.Add((pair.attribute.TargetType, pair.attribute.RequiredAttribute), pair.drawerType);
            }

            foreach (var type in TypeReader.FindAllTypesWhere(type => !type.IsAbstract && typeof(IRuntimeDrawerDecorator).IsAssignableFrom(type)))
            {
                var decorator = (IRuntimeDrawerDecorator)Activator.CreateInstance(type);
                DecoratorDict.Add(decorator.RequiredAttribute, decorator);
            }

            bgHeightLightTransition = new RSTransition();
            bgHeightLightTransition.AnimationMode = RSAnimationMode.Backward;
            bgHeightLightTransition.AddTransition(0f, new RSStyle() { Background = new RSBackground { color = Color.clear } });
            bgHeightLightTransition.AddTransition(0.7f, new RSStyle() { Background = new RSBackground { color = new Color(.35f, 1f, .35f, .35f) } });
        }
        private static readonly Dictionary<Type, CustomRuntimeDrawerAttribute> drawerType2Attribute = new();
        private static readonly Dictionary<(Type valueType, Type attrType), Type> drawerMatchDict = new();

        public static CustomRuntimeDrawerAttribute GetDrawerAttribute(Type drawerType)
        {
            CustomRuntimeDrawerAttribute attr;
            if (drawerType2Attribute.TryGetValue(drawerType, out attr))
                return attr;
            return null;
        }
        public static Type FindDrawerType(Type dataType, IEnumerable<Type> attrTypes)
        {
            Type result;
            foreach(var attrType in attrTypes)
                if(drawerMatchDict.TryGetValue((dataType, attrType), out result))
                    return result;
            return FindDrawerType(dataType);
        }
        public static Type FindDrawerType(Type dataType, Type attrType)
        {
            if (drawerMatchDict.TryGetValue((dataType, attrType), out var result))
                return result;
            return FindDrawerType(dataType);
        }
        public static Type FindDrawerType(Type dataType)
        {
            (Type,Type) key = (dataType, null);
            if (drawerMatchDict.TryGetValue(key, out var result))
                return result;
            foreach(var pair in ActiveDrawer)
            {
                if (pair.attribute.CanDrawType(dataType))
                {
                    result = pair.drawerType;
                    break;
                }
            }
            result ??= typeof(DefaultDrawer);
            drawerMatchDict.Add(key, result);
            return result;
        }
        public static void DecorateDrawer(RuntimeDrawer drawer, IEnumerable<Attribute> attributes)
        {
            IRuntimeDrawerDecorator decorator;
            foreach(var attribute in attributes)
            {
                if (DecoratorDict.TryGetValue(attribute.GetType(), out decorator))
                    decorator.DecorateDrawer(attribute, drawer);
            }
        }
        public static RuntimeDrawer CreateFromMember(Type type, string memberName)
        {
            return CreateFromMember(type.GetMember(memberName, BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)[0]);
            
        }
        public static RuntimeDrawer CreateFromMember(MemberInfo memberInfo)
        {
            return RuntimeDrawerFactory
                .FromValue(memberInfo.FieldOrPropertyType())
                .AddAttribute(memberInfo)
                .Label(DefaultDrawer.ProcessName(memberInfo.Name))
                .Build();
        }
        public static RuntimeDrawer Create(object value, string label = "")
        {
            return RuntimeDrawerFactory.FromValue(value).Label(label).Build();
        }
        public static RuntimeDrawer Create(object value, string label = "", params Attribute[] attributes)
        {
            return RuntimeDrawerFactory.FromValue(value).Label(label).AddAttribute(attributes).Build();
        }

        private static RSTransition bgHeightLightTransition;
        private static Dictionary<VisualElement, RSTransitionPlayer> playingVe = new ();
        private static void changeChaeck(RuntimeDrawer evt)
        {
            if(playingVe.TryGetValue(evt, out var player))
            {
                player.Start();
                return;
            }
            bgHeightLightTransition.Properties[0].Background.LoadFrom(evt);
            var transitionPlayer = bgHeightLightTransition.MakePlayerByCopy((evt));
            transitionPlayer.Start();
            playingVe.Add(evt, transitionPlayer);
            transitionPlayer.OnTransitionEnd += () => { playingVe.Remove(evt); };
        }
        public static void BeginChangeDebug(VisualElement visualTree)
        {
            foreach (var drawer in visualTree.ChildrenRecursive<RuntimeDrawer>())
            {
                drawer.OnValueChanged += () => changeChaeck(drawer);
                drawer.OnMemberValueChanged += _ => changeChaeck(drawer);
            }
        }
        private static void _AddPropertiesTooltips(RuntimeDrawer drawer)
        {
            drawer.tooltipElement.Add(new RSTypeNameElement(drawer.GetType()));
            drawer.tooltipElement.Add(new RSTextElement($"Nest {drawer.NestLevel}"));
        }
        public static void Debug_AddPropertiesTooltips(VisualElement visualTree)
        {
            foreach (var drawer in visualTree.ChildrenRecursive<RuntimeDrawer>(true))
            {
                _AddPropertiesTooltips(drawer);
                drawer.tooltipElement.PopupDelay = 10;
            }
        }
        #endregion
        //public RuntimeDrawerLayoutMode LayoutMode
        //{
        //    get => m_LayoutMode;
        //    set
        //    {
        //        switch (value)
        //        {
        //            case RuntimeDrawerLayoutMode.Inline:
        //                LayoutInline();
        //                break;
        //            case RuntimeDrawerLayoutMode.Expand:
        //                LayoutExpand();
        //                break;
        //        }
        //    }
        //}
        //RuntimeDrawerLayoutMode m_LayoutMode;
        public override VisualElement contentContainer => m_container;
        VisualElement m_container;

        public RSLocalizeText localizeLabel
        {
            get
            {
                labelElement.localizeText ??= new();
                return labelElement.localizeText;
            }
            set
            {
                labelElement.localizeText = value;
                labelElement.ReloadText();
                updateLabelState();
            }
        }
        public readonly RSLocalizeTextElement labelElement;
        public readonly VisualElement iconElement;
        public TooltipElement tooltipElement
        {
            get {
                if(m_tooltipElement == null)
                {
                    tooltipElement = new TooltipElement(this);
                    m_tooltipElement.style.backgroundColor = RSTheme.Current.BackgroundColor;
                    m_tooltipElement.style.SetRS_Style(new RSBorder() { anyColor = RSTheme.Current.FrontgroundColor, anyWidth = 1.5f });
                    m_tooltipElement.style.SetRS_Style(new RSPadding { any = RSTheme.Current.VisualMargin });
                }
                return m_tooltipElement;
            }
            set
            {
                if(value != m_tooltipElement)
                {
                    if(m_tooltipElement != null)
                        m_tooltipElement.UnregisterPopupOnTarget(labelElement);
                    m_tooltipElement = value;
                    m_tooltipElement.RegisterPopupOnTarget(labelElement);
                }
            }
        }
        private TooltipElement m_tooltipElement;
        public string label
        {
            get => localizeLabel.value;
            set
            {
                localizeLabel.value = value;
                labelElement.ReloadText();
                updateLabelState();
            }
        }
        void updateLabelState()
        {
            if(labelElement.text == "")
                labelElement.style.display = DisplayStyle.None;
            else
                labelElement.style.display = DisplayStyle.Flex;
        }
        public float labelWidth
        {
            get => labelElement.style.width.value.value;
            set
            {
                labelElement.style.width = value;
            }
        }
        public float indentWidth
        {
            get => style.paddingLeft.value.value;
            set
            {
                style.paddingLeft = value;
                iconElement.style.left = -RSTheme.Current.LineHeight + value;
            }
        }
        public bool enableIcon
        {
            get => iconElement.style.display == DisplayStyle.Flex;
            set => iconElement.style.display = value ? DisplayStyle.Flex : DisplayStyle.None;
        }
        /// <summary>
        /// You can also use as IndentLevel
        /// </summary>
        public readonly int NestLevel;
        /// <summary>
        /// Computed LabelWidth from RuntimeDrawer construct
        /// </summary>
        public readonly float MeasuredLabelWidth;
        /// <summary>
        /// Computed IndentWidth from RuntimeDrawer construct
        /// </summary>
        public readonly float MeasuredIndentWidth;
        public virtual bool DynamicLayout => false;

        public event Action OnValueChanged;
        public event Action<RuntimeDrawer> OnMemberValueChanged;

        public RuntimeDrawer()
        {
            style.flexShrink = 0;
            NestLevel = RSTheme.indentLevel;
            RSTheme.indentLevel++;

            m_container = new VisualElement();
            m_container.name = "rd-container";
            labelElement = new RSLocalizeTextElement()
            {
                name = "rd-label",
                style =
                {
                    height = RSTheme.Current.LineHeight,
                    unityTextAlign = TextAnchor.MiddleLeft,
                    textOverflow = TextOverflow.Ellipsis,
                    whiteSpace = WhiteSpace.NoWrap,
                    flexGrow = 1,
                    paddingLeft = RSTheme.Current.VisualMargin,
                }
            };
            iconElement = new VisualElement()
            {
                name = "rd-icon",
                style =
                {
                    width = RSTheme.Current.LineHeight,
                    height = RSTheme.Current.LineHeight,
                    position = Position.Absolute,
                }
            };

            MeasuredLabelWidth = RSTheme.Current.GetIndentedLabelWidth(NestLevel);
            MeasuredIndentWidth = RSTheme.Current.GetIndentWidth(NestLevel);

            labelWidth = MeasuredLabelWidth;
            indentWidth = MeasuredIndentWidth;
            hierarchy.Add(iconElement);
            hierarchy.Add(labelElement);
            hierarchy.Add(contentContainer);

            label = "";
            if (NestLevel > MaxNestLevel)
            {
                var msg = $"Runtime Drawer try to layout nest more than {MaxNestLevel}. Check if it's a recursive problem or modify MaxNestLevel.";
                throw new Exception(msg);
            }

            LayoutInline();
            if (!DynamicLayout)
                CreateGUI();
            RSTheme.indentLevel--;
        }
        protected void InvokeValueChange() { OnValueChanged?.Invoke(); }
        protected void InvokeMemberValueChange(RuntimeDrawer drawer) { OnMemberValueChanged?.Invoke(drawer); }
        public virtual void LayoutInline()
        {
            style.flexDirection = FlexDirection.Row;
            labelElement.style.width = MeasuredLabelWidth - MeasuredIndentWidth;
            labelElement.style.flexGrow = 0;
            m_container.style.flexGrow = 1;
        }
        public virtual void LayoutExpand()
        {
            style.flexDirection = FlexDirection.Column;
            labelElement.style.width = StyleKeyword.Auto;
            labelElement.style.flexGrow = 1;
            m_container.style.flexGrow = 0;
            MarkDirtyRepaint();
        }

        public abstract object GetValue();
        public abstract void SetValue(object value);
        public abstract void SetValueWithoutNotify(object newValue);
        public virtual void ReciveAttribute(Attribute attribute)
        {
            throw new NotImplementedException($"ReciveAttribute() Not Implement on Creating \"{TypeReader.GetName(GetType())}\" with \"{TypeReader.GetName(attribute.GetType())}\"");
        }
        protected abstract void CreateGUI();
        public abstract void RepaintDrawer();

        public void SetIcon(Sprite sprite) { SetIcon(Background.FromSprite(sprite)); }
        public void SetIcon(Texture2D texture2D) { SetIcon(Background.FromTexture2D(texture2D)); }
        public virtual void SetIcon(Background img)
        {
            iconElement.style.backgroundImage = img;
        }
        /// This use reflection to bind value, if the performence is importent, you may want to do it your self.
        /// </summary>
        public bool Bind(object obj, string path)
        {
            var objType = obj.GetType();
            if (!objType.IsClass)
                throw new Exception("You can only bind value on RenferenceType (class)");
            MemberInfo[] members = objType.GetMember(path, BindingFlags.GetProperty | BindingFlags.GetField | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(var member in members)
            {
                switch (member)
                {
                    case PropertyInfo :
                    case FieldInfo :
                        var fieldType = member.FieldOrPropertyType();
                        var setter = DefaultDrawer.GetSetter(member);
                        try
                        {
                            setter(obj, DefaultDrawer.GetGetter(member)(obj));
                        }
                        catch
                        {
                            throw new Exception($"You can't bind {TypeReader.GetName(fieldType)} value on a field isn't {TypeReader.GetName(fieldType)}.");
                        }
                        OnValueChanged += () => setter(obj, GetValue());
                        return true;
                    default:
                        return false;
                }
            }
            throw new Exception($"Not found member {path} in value");
        }
    }
    public abstract class RuntimeDrawer<T> : RuntimeDrawer
    {
        public T value
        {
            get => m_value; 
            set => SetValue(value);
        }
        protected T m_value;

        private void setValueCaller(T newValue, bool isNotify, bool isUpdate)
        {
            bool hasChanged = !m_value?.Equals(newValue) ?? !newValue?.Equals(m_value) ?? false;
            if (isNotify && hasChanged)
            {
                m_value = newValue;
                InvokeValueChange();
            }
            else if (hasChanged)
            {
                m_value = newValue;
            }
            if (hasChanged && DynamicLayout)
            {
                var originIndent = RSTheme.indentLevel;
                RSTheme.indentLevel = NestLevel + 1;
                CreateGUI();
                RSTheme.indentLevel = originIndent;
            }
            if (isUpdate)
                RepaintDrawer();
        }
        public override object GetValue()
        {
            return m_value;
        }
        public override void SetValue(object value) { SetValue((T)value); }
        public void SetValue(T newValue) { setValueCaller(newValue, true, true); }
        public override void SetValueWithoutNotify(object newValue) { SetValueWithoutNotify((T)newValue); }
        public void SetValueWithoutNotify(T newValue) { setValueCaller(newValue, false, true); }
        public void SetValueWithoutRepaint(T newValue) { setValueCaller(newValue, true, false); }

    }

    public sealed class RuntimeDrawerFactory
    {
        private static RuntimeDrawerFactory factory = new RuntimeDrawerFactory();
        private RuntimeDrawerFactory() { }

        private static List<Attribute> m_attributes = new();
        private static HashSet<Type> m_attributeTypes = new();
        private static Type m_drawerType;
        private static Type m_valueType;
        private static object m_value;
        private static string m_label = "";
        public static void Clear()
        {
            m_attributes.Clear();
            m_attributeTypes.Clear();
            m_drawerType = null;
            m_valueType = null;
            m_value = null;
            m_label = "";
        }
        public static RuntimeDrawerFactory FromValue(object value)
        {
            Clear();
            m_value = value;
            return factory;
        }
        public static RuntimeDrawerFactory FromValueType(Type type)
        {
            Clear();
            m_valueType = type;
            return factory;
        }
        public static RuntimeDrawerFactory FromDrawerType(Type type)
        {
            Clear();
            m_drawerType = type;
            return factory;
        }
        public RuntimeDrawerFactory Label(string label)
        {
            m_label = label;
            return this;
        }
        public RuntimeDrawerFactory AddAttribute(MemberInfo member) { return AddAttribute(member.GetCustomAttributes()); }
        public RuntimeDrawerFactory AddAttribute(IEnumerable<Attribute> attribute)
        {
            if (attribute == null) return this;
            m_attributes.AddRange(attribute);
            foreach(var attr in attribute)
                m_attributeTypes.Add(attr.GetType());
            return this;
        }
        public RuntimeDrawerFactory AddAttribute(Attribute attribute)
        {
            m_attributes.Add(attribute);
            m_attributeTypes.Add(attribute.GetType());
            return this;
        }
        public RuntimeDrawer Build()
        {
            if (m_drawerType == null)
            {
                if (m_valueType == null)
                {
                    if (m_value == null)
                    {
                        Debug.LogError("[RuntimeDrawerFactory] ValueObject / ValueType / DrawerType Not Found.");
                        return null;
                    }
                    m_valueType = m_value.GetType();
                }
                if (m_attributes.Count > 0)
                    m_drawerType = RuntimeDrawer.FindDrawerType(m_valueType, m_attributeTypes);
                else
                    m_drawerType = RuntimeDrawer.FindDrawerType(m_valueType);
            }
            return build(m_drawerType, m_label, m_value, m_attributes.ToArray());
        }
        private RuntimeDrawer build(Type drawerType, string label, object value, Attribute[] attributes)
        {
            var drawer = (RuntimeDrawer)Activator.CreateInstance(drawerType);
            drawer.label = label;
            if (value != null)
                drawer.SetValue(value);
            var attrInfo = RuntimeDrawer.GetDrawerAttribute(drawerType);
            if (attrInfo.RequiredAttribute != null)
                drawer.ReciveAttribute(m_attributes.Find(attr => attr.GetType() == attrInfo.RequiredAttribute));
            if (m_attributes.Count > 0)
                RuntimeDrawer.DecorateDrawer(drawer, m_attributes);
            return drawer;
        }
    }
}