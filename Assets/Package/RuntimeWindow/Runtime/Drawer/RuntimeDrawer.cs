using NaiveAPI.DocumentBuilder;
using NaiveAPI_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UIElements;

namespace NaiveAPI.RuntimeWindowUtils
{
    public interface IRuntimeDrawer
    {
        public void SetValue(object value);
        public object GetValue();
        public void SetValueWithoutNotify(object newValue);
        public void UpdateField();
        public void ReciveAttribute(Attribute attribute);
        public void RegisterValueChangedAsEventBase(EventCallback<EventBase> eventCallback);
        public void UnRegisterValueChangedAsEventBase(EventCallback<EventBase> eventCallback);
    }
    public interface IRuntimeDrawerDecorator
    {
        public Type RequiredAttribute { get; }
        public void DecorateDrawer(Attribute attribute, RuntimeDrawer drawer);
    }
    public abstract class RuntimeDrawer : VisualElement, IRuntimeDrawer
    {
        #region static
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
        public static RuntimeDrawer Create(object value, string label = "")
        {
            return RuntimeDrawerFactory.FromObject(value).Label(label).Build();
        }
        public static RuntimeDrawer Create(object value, string label = "", params Attribute[] attributes)
        {
            return RuntimeDrawerFactory.FromObject(value).Label(label).AddAttribute(attributes).Build();
        }
        #endregion

        public override VisualElement contentContainer => m_container;
        VisualElement m_container;

        public readonly DSTextElement labelElement;
        public readonly VisualElement iconElement;
        public readonly VisualElement titleElement;
        public string label
        {
            get => labelElement.text;
            set
            {
                if (string.IsNullOrEmpty(value))
                    labelElement.style.display = DisplayStyle.None;
                else
                {
                    labelElement.style.display = DisplayStyle.Flex;
                    (labelElement as INotifyValueChanged<string>).SetValueWithoutNotify(value);
                }
            }
        }
        public virtual bool DynamicLayout => false;
        public RuntimeDrawer()
        {
            m_container = new VisualElement();
            labelElement = new DSTextElement();
            labelElement.style.width = DocStyle.Current.LabelWidth;
            labelElement.style.height = DocStyle.Current.LineHeight;
            labelElement.style.textOverflow = TextOverflow.Ellipsis;
            labelElement.style.whiteSpace = WhiteSpace.NoWrap;
            iconElement = new VisualElement();
            iconElement.style.SetIS_Style(DocStyle.Current.IconStyle);
            titleElement = new DSHorizontal();
            titleElement.Add(iconElement);
            titleElement.Add(labelElement);
            hierarchy.Add(titleElement);

            label = "";
            LayoutInline();
            if(!DynamicLayout)
                OnCreateGUI();
        }

        public virtual void LayoutInline()
        {
            m_container.style.marginLeft = 0;
            m_container.style.flexGrow = 1;
            labelElement.style.flexGrow = 0;
            titleElement.Add(m_container);
        }
        public virtual void LayoutExpand()
        {
            m_container.style.marginLeft = DocStyle.Current.LineHeight;
            m_container.style.flexGrow = 0;
            labelElement.style.flexGrow = 1;
            hierarchy.Add(m_container);
        }

        public abstract object GetValue();
        public abstract void SetValue(object value);
        public abstract void SetValueWithoutNotify(object newValue);
        public virtual void ReciveAttribute(Attribute attribute)
        {
            throw new NotImplementedException($"ReciveAttribute() Not Implement on Creating \"{TypeReader.GetName(GetType())}\" with \"{TypeReader.GetName(attribute.GetType())}\"");
        }
        public abstract void RegisterValueChangedAsEventBase(EventCallback<EventBase> eventCallback);
        public abstract void UnRegisterValueChangedAsEventBase(EventCallback<EventBase> eventCallback);
        public abstract void UpdateField();
        protected abstract void OnCreateGUI();

    }
    public abstract class RuntimeDrawer<T> : RuntimeDrawer, INotifyValueChanged<T>
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
                using ChangeEvent<T> evt = ChangeEvent<T>.GetPooled(m_value, newValue);
                evt.target = this;
                m_value = newValue;
                SendEvent(evt);
            }
            else if (hasChanged)
            {
                m_value = newValue;
            }
            if (hasChanged && DynamicLayout)
                OnCreateGUI();
            if (isUpdate)
                UpdateField();
        }
        public override object GetValue()
        {
            return m_value;
        }
        public override void SetValue(object value) { SetValue((T)value); }
        public void SetValue(T newValue) { setValueCaller(newValue, true, true); }
        public override void SetValueWithoutNotify(object newValue) { SetValueWithoutNotify((T)newValue); }
        public void SetValueWithoutNotify(T newValue) { setValueCaller(newValue, false, true); }
        public void SetValueWithoutUpdate(T newValue) { setValueCaller(newValue, true, false); }
        public override void RegisterValueChangedAsEventBase(EventCallback<EventBase> eventCallback)
        {
            this.RegisterValueChangedCallback(eventCallback);
        }
        public override void UnRegisterValueChangedAsEventBase(EventCallback<EventBase> eventCallback)
        {
            this.RegisterValueChangedCallback(eventCallback);
        }
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
        public static RuntimeDrawerFactory FromObject(object obj)
        {
            Clear();
            m_value = obj;
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
            if(m_drawerType == null)
            {
                if(m_valueType == null)
                {
                    if(m_value == null)
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
                drawer.SetValueWithoutNotify(value);
            var attrInfo = RuntimeDrawer.GetDrawerAttribute(drawerType);
            if (attrInfo.RequiredAttribute != null)
                drawer.ReciveAttribute(m_attributes.Find(attr => attr.GetType() == attrInfo.RequiredAttribute));
            if (m_attributes.Count > 0)
                RuntimeDrawer.DecorateDrawer(drawer, m_attributes);
            return drawer;
        }
    }
}