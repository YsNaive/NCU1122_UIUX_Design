using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSPropertyInfoElement : RSScriptAPIElement
    {
        public PropertyInfo Target => m_Target;
        private PropertyInfo m_Target;
        public RSTextElement AreaText => m_AreaText;
        private RSTextElement m_AreaText;
        public RSTypeNameElement TypeText => m_TypeText;
        private RSTypeNameElement m_TypeText;
        public RSTextElement NameText => m_NameText;
        private RSTextElement m_NameText;
        public RSPropertyInfoElement(PropertyInfo propertyInfo)
            : base()
        {
            m_Target = propertyInfo;
            var padding = RSTheme.Current.LineHeight/2f;
            var getText = new RSTextElement("get");
            getText.style.color = RSTheme.Current.CSharp.prefixColor;
            getText.style.opacity = propertyInfo.CanRead ? 1f : 0.4f;

            var setText = new RSTextElement("set");
            setText.style.color = RSTheme.Current.CSharp.prefixColor;
            setText.style.opacity = propertyInfo.CanWrite ? 1f : 0.4f;
            setText.style.marginLeft = padding;

            m_TypeText = new RSTypeNameElement(propertyInfo.PropertyType);
            m_TypeText.style.color = RSTheme.Current.CSharp.classColor;
            m_TypeText.style.marginLeft = padding;

            m_NameText = new RSTextElement(propertyInfo.Name);
            m_NameText.style.color = RSTheme.Current.CSharp.parameterColor;
            m_NameText.style.marginLeft = padding;
            Add(getText);
            Add(setText);
            Add(m_AreaText);
            Add(m_TypeText);
            Add(m_NameText);
        }

        public override IEnumerable<RSTypeNameElement> VisitTypeName()
        {
            foreach(var ve in m_TypeText.VisitTypeName())
                yield return ve;
        }

        public override IEnumerable<RSParameterInfoElement> VisitParameter()
        {
            return Enumerable.Empty<RSParameterInfoElement>();
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            yield return (Target, NameText);
        }
    }
}
