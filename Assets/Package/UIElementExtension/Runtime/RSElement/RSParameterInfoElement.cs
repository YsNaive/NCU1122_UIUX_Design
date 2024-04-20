using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSParameterInfoElement : RSScriptAPIElement
    {
        public ParameterInfo Target => m_Target;
        private ParameterInfo m_Target;
        public RSTypeNameElement TypeText => m_TypeText;
        private RSTypeNameElement m_TypeText;
        public RSTextElement NameText => m_NameText;
        private RSTextElement m_NameText;
        public RSParameterInfoElement(ParameterInfo paramInfo)
            : base()
        {
            m_Target = paramInfo;
            var padding = RSTheme.Current.LineHeight/2f;

            m_TypeText = new RSTypeNameElement(paramInfo.ParameterType);

            m_NameText = new RSTextElement(paramInfo.Name);
            m_NameText.style.color = RSTheme.Current.CSharp.parameterColor;
            m_NameText.style.marginLeft = padding;

            RSTextElement areaText = new();
            if (paramInfo.IsIn)
                areaText = new RSTextElement("in");
            else if(paramInfo.IsOut)
                areaText = new RSTextElement("out");
            if(areaText.text != "")
            {
                areaText.style.color = RSTheme.Current.CSharp.prefixColor;
                Add(areaText);
                m_TypeText.style.marginLeft = padding;
            }

            Add(m_TypeText);
            Add(m_NameText);
        }

        public override IEnumerable<RSTypeNameElement> VisitTypeName()
        {
            foreach (var ve in m_TypeText.VisitTypeName())
                yield return ve;
        }

        public override IEnumerable<RSParameterInfoElement> VisitParameter()
        {
            yield return this;
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            yield return (Target, m_NameText);
        }
    }

}