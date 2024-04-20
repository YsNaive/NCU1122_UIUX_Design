using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSFieldInfoElement : RSScriptAPIElement
    {
        public FieldInfo Target => m_Target;
        private FieldInfo m_Target;
        public RSTypeNameElement TypeText => m_TypeText;
        private RSTypeNameElement m_TypeText;
        public RSTextElement NameText => m_NameText;
        private RSTextElement m_NameText;
        public RSFieldInfoElement(FieldInfo fieldInfo)
            : base()
        {
            m_Target = fieldInfo;
            //var areaText = new RSTextElement(TypeReader.GetAccessLevel(fieldInfo));
            //areaText.style.color = DocStyle.Current.PrefixColor;

            m_TypeText = new RSTypeNameElement(fieldInfo.FieldType);
            //m_TypeText.style.marginLeft = RSTheme.Current.LineHeight/2f;

            m_NameText = new RSTextElement(fieldInfo.Name);
            m_NameText.style.color = RSTheme.Current.CSharp.parameterColor;
            m_NameText.style.marginLeft = RSTheme.Current.LineHeight/2f;

            //Add(areaText);
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
            return Enumerable.Empty<RSParameterInfoElement>();
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            yield return (Target, m_NameText);
        }
    }

}