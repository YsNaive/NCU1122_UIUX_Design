using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSMethodInfoElement : RSScriptAPIElement
    {
        public MethodBase Target => m_Target;
        private MethodBase m_Target;
        public RSTypeNameElement ReturnTypeText => m_ReturnTypeText;
        private RSTypeNameElement m_ReturnTypeText;
        public RSTypeNameElement[] GenericTypeText => m_GenericTypeText;
        private RSTypeNameElement[] m_GenericTypeText;
        public RSTextElement NameText => m_NameText;
        private RSTextElement m_NameText;
        public RSParameterInfoElement[] ParamTexts => m_ParamTexts;
        private RSParameterInfoElement[] m_ParamTexts;
        public RSMethodInfoElement(MethodBase info)
            : base()
        {
            m_Target = info;
            style.flexWrap = Wrap.Wrap;
            var padding = RSTheme.Current.LineHeight/2f;
            //var areaText = new RSTextElement(TypeReader.GetAccessLevel(info));
            //areaText.style.color = DocStyle.Current.PrefixColor;

            m_ReturnTypeText = new RSTypeNameElement(info switch
            {
                MethodInfo asMethod => asMethod.ReturnType,
                ConstructorInfo asConstructor => asConstructor.ReflectedType,
                _ => null
            });
            //m_ReturnTypeText.style.marginLeft = padding;


            var paramInfos = info.GetParameters();
            m_ParamTexts = new RSParameterInfoElement[paramInfos.Length];
            for (int i = 0; i < paramInfos.Length; i++)
                m_ParamTexts[i] = new RSParameterInfoElement(paramInfos[i]);

            //Add(areaText);
            Add(m_ReturnTypeText);
            if (!info.IsConstructor)
            {
                m_NameText = new RSTextElement(info.Name);
                m_NameText.style.color = RSTheme.Current.CSharp.methodColor;
                m_NameText.style.marginLeft = padding;
                Add(m_NameText);
                if (info.IsGenericMethod)
                {
                    Add(new RSTextElement("<"));
                    var margin = RSTheme.Current.LineHeight/2f;
                    var args = info.GetGenericArguments();
                    m_GenericTypeText = new RSTypeNameElement[args.Length];
                    var i = 0;
                    foreach (var t in args)
                    {
                        m_GenericTypeText[i] = new RSTypeNameElement(t);
                        Add(m_GenericTypeText[i]);
                        i++;
                        var element = new RSTextElement(",");
                        element.style.marginRight = margin;
                        Add(element);
                    }
                    var last = ((RSTextElement)this[childCount - 1]);
                    last.text = ">";
                    last.style.marginRight = 0;
                }

            }
            Add(new RSTextElement("("));
            if(m_ParamTexts.Length != 0)
            {
                Add(m_ParamTexts[0]);
                for (int i = 1; i < paramInfos.Length; i++)
                {
                    var comma = new RSTextElement(",");
                    var param = m_ParamTexts[i];
                    param.style.marginLeft = padding;
                    Add(comma);
                    Add(param);
                }
            }
            Add(new RSTextElement(")"));
        }

        public override IEnumerable<RSTypeNameElement> VisitTypeName()
        {
            if (!m_Target.IsConstructor)
            {
                foreach (var ve in m_ReturnTypeText.VisitTypeName())
                    yield return ve;
            }
            foreach(var param in m_ParamTexts)
            {
                foreach (var ve in param.VisitTypeName())
                    yield return ve;
            }
            if (m_GenericTypeText != null)
            {
                foreach (var param in m_GenericTypeText)
                {
                    foreach (var ve in param.VisitTypeName())
                        yield return ve;
                }
            }
        }

        public override IEnumerable<RSParameterInfoElement> VisitParameter()
        {
            foreach(var param in m_ParamTexts)
                yield return param;
        }

        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            yield return (Target, (Target.IsConstructor ? m_ReturnTypeText : m_NameText));
            foreach (var param in m_ParamTexts)
            {
                foreach (var it in param.VisitMember())
                    yield return it;
            }
        }
    }
}