using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UIElements;

namespace NaiveAPI.UITK
{
    public abstract class RSScriptAPIElement : RSHorizontal
    {
        public abstract IEnumerable<RSTypeNameElement> VisitTypeName();
        public abstract IEnumerable<RSParameterInfoElement> VisitParameter();
        public abstract IEnumerable<(ICustomAttributeProvider memberInfo, VisualElement element)> VisitMember();
        public static RSScriptAPIElement Create(ICustomAttributeProvider info)
        {
            return info switch
            {
                FieldInfo asField => new RSFieldInfoElement(asField),
                PropertyInfo asProp => new RSPropertyInfoElement(asProp),
                MethodInfo asMethod => new RSMethodInfoElement(asMethod),
                ConstructorInfo asCtor => new RSMethodInfoElement(asCtor),
                ParameterInfo asParam => new RSParameterInfoElement(asParam),
                _ => null
            };
        }
    }
}
