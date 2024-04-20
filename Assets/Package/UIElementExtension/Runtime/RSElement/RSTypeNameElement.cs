using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public class RSTypeNameElement : RSScriptAPIElement
    {
        public Type TargetType => m_TargetType;
        private Type m_TargetType;
        public RSTextElement NameText => m_NameText;
        private RSTextElement m_NameText;
        private RSTypeNameElement[] genericTypeName;
        public RSTypeNameElement(Type type)
            : base()
        {
            m_TargetType = type;
            m_NameText = new RSTextElement();
            if (TypeReader.TypeNameTable.ContainsKey(type))
                m_NameText.style.color = RSTheme.Current.CSharp.prefixColor;
            else if (type.IsValueType || type.IsInterface || type.IsEnum)
                m_NameText.style.color = RSTheme.Current.CSharp.structColor;
            else
                m_NameText.style.color = RSTheme.Current.CSharp.classColor;
            Add(m_NameText);

            if (type.IsGenericType)
            {
                var i = type.Name.IndexOf('`');
                if (i != -1) m_NameText.text = type.Name.Substring(0, i);
                else m_NameText.text = type.Name;
                Add(new RSTextElement("<"));
                var margin = RSTheme.Current.LineHeight/2f;
                var args = type.GetGenericArguments();
                genericTypeName = new RSTypeNameElement[args.Length];
                i = 0;
                foreach (var t in args)
                {
                    genericTypeName[i] = new RSTypeNameElement(t);
                    Add(genericTypeName[i]);
                    i++;
                    var element = new RSTextElement(",");
                    element.style.marginRight = margin;
                    Add(element);
                }
                var last = ((RSTextElement)this[childCount - 1]);
                last.text = ">";
                last.style.marginRight = 0;
            }
            else
            {
                m_NameText.text = TypeReader.GetName(type);
            }

        }

        public override IEnumerable<RSTypeNameElement> VisitTypeName()
        {
            yield return this;
            if (genericTypeName != null)
            {
                foreach (var element in genericTypeName)
                {
                    foreach (var subElement in element.VisitTypeName())
                        yield return subElement;
                }
            }
        }

        public override IEnumerable<RSParameterInfoElement> VisitParameter()
        {
            return Enumerable.Empty<RSParameterInfoElement>();
        }
        public override IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember()
        {
            return Enumerable.Empty<(ICustomAttributeProvider, VisualElement)>();
        }
    }
}
